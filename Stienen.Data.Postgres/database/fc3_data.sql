CREATE TABLE device(
	"Id" uuid,
	"Gid" uuid not null,
	"Address" int not null,
	"Module" int not null,
	"Name" varchar(31) not null,
	"Descr" varchar(255),
	"Active" bool not null,
	"Order" int default 1,
	CONSTRAINT PK_Devices PRIMARY KEY ("Id")
);
CREATE unique index "UX_Devices" ON "Devices" ("Gid", "Address", "Module");

CREATE TABLE raw_device_data (
	id serial primary key,
	device_id uuid not null,
	index int not null,
	stamp timestamp not null,
	data bytea not null
);

CREATE TABLE "FE_References"(
	"Hardware" int,
	"Version" int,
	"Id" int,
	"Index" int not null,
	"Length" int not null,
	"Name" varchar(31) not null,
	"Type" int not null,
	"Defop" int null,
	CONSTRAINT PK_FE_References PRIMARY KEY ("Hardware", "Version", "Id")
);
CREATE index "IX_FE_RefIndex" ON "FE_References" ("Hardware", "Version", "Index");
CREATE unique index "UX_FE_RefName" ON "FE_References" ("Hardware", "Version", "Name");

CREATE TABLE "FE_Types"(
	"Hardware" int,
	"Version" int,
	"Id" int,
	"Name" varchar(31) not null,
	"Type" int not null,
	"Mul" int not null,
	"Div" int not null,
	"Step" float not null,
	"Min" int not null,
	"Max" int not null,
	"Text" int null,
	"AckChange" boolean DEFAULT false,
	CONSTRAINT PK_FE_Types PRIMARY KEY ("Hardware", "Version", "Id"),
	CONSTRAINT CK_FE_Types CHECK("Min"<="Max")
);

CREATE TABLE public.device_data
(
  did uuid NOT NULL,
  index integer NOT NULL,
  stamp timestamp without time zone NOT NULL,
  data bytea NOT NULL,
  CONSTRAINT values_pkey PRIMARY KEY (did, index, stamp)
)
WITH (
  OIDS=FALSE,
  autovacuum_enabled=true,
  autovacuum_vacuum_threshold=10000,
  autovacuum_vacuum_scale_factor=0.0,
  autovacuum_analyze_threshold=10000,
  autovacuum_analyze_scale_factor=0.0,
  toast.autovacuum_enabled=true
);
ALTER TABLE public.device_data
  OWNER TO postgres;

CREATE TABLE public.device_data_insert_table
(
  did uuid NOT NULL,
  hardware integer NOT NULL,
  version integer NOT NULL,
  stamp timestamp without time zone NOT NULL,
  drift integer,
  index integer NOT NULL,
  data bytea NOT NULL,
  store_type integer NOT NULL
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.device_data_insert_table
  OWNER TO postgres;

CREATE OR REPLACE VIEW public.device_data_insert_view
  as select * from public.device_data_insert_table;

CREATE TABLE public.device_data_jsonb
(
  id serial primary key,
  device_id uuid not null,
  stamp date not null,
  name character varying not null,
  data jsonb
)
WITH (
  OIDS=FALSE,
  autovacuum_vacuum_threshold=250,
  autovacuum_vacuum_scale_factor=0.0,
  autovacuum_analyze_threshold=500,
  autovacuum_analyze_scale_factor=0.0
);
ALTER TABLE public.device_data_jsonb
  OWNER TO postgres;


CREATE UNIQUE INDEX ux_device_data_jsonb_did_name_stamp
  ON public.device_data_jsonb
  USING btree
  (device_id, index, stamp);

CREATE TABLE public.trigger_error
(
  id serial primary key,
  stamp timestamp without time zone NOT NULL,
  event text NOT NULL,
  message text,
  code text NOT NULL,
  context text
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.trigger_error
  OWNER TO postgres;

CREATE INDEX "IDX_error_stamp"
  ON public.trigger_error
  USING btree
  (stamp);

-- optimize this to be a table instead of a view, or make it a materialized view that updates on new inserts off dependency tables
CREATE OR REPLACE VIEW public.refs_backend AS
 SELECT 
	fr."Hardware" as hardware,
    fr."Version" as version,
    fr."Index" as index,
    fr."Length" as length,
    fr."Name" as name,
    ft."Type" as type,
    ft."Mul" as mul,
    ft."Div" as div
FROM "FE_References" fr
   JOIN "FE_Types" ft ON ft."Hardware" = fr."Hardware" AND ft."Version" = fr."Version" AND ft."Id" = fr."Type"
WHERE fr."Index" >= 0;

-- DROP FUNCTION IF EXISTS SignNumber("value" bigint, "type" int);
CREATE OR REPLACE FUNCTION SignNumber("value" bigint, "type" int)
RETURNS bigint
AS $$
	SELECT CASE
		WHEN $2=8  AND $1>x'7F'::int  THEN ($1-x'FF'::int)-1  -- T_I8
		WHEN $2=10 AND $1>x'7FFF'::int THEN ($1-x'FFFF'::int)-1 -- T_I16
		WHEN $2=12 AND $1>x'7FFFFFFF'::int THEN ($1-x'FFFFFFFF'::bigint)-1 -- T_I32
		--WHEN $2=14 AND $1>x'7FFFFFFFFFFFFFFF'::bigint THEN ($1-x'FFFFFFFFFFFFFFFF'::bigint)-1 -- T_I64
		ELSE $1
	END;
$$ LANGUAGE SQL IMMUTABLE;

CREATE OR REPLACE FUNCTION Overlap(sl int, el int, sr int, er int)
RETURNS int
AS $$
	SELECT LEAST($2, $4)-GREATEST($1,$3);
$$ LANGUAGE SQL IMMUTABLE;

-- DROP FUNCTION IF EXISTS DataAt(index int, buffer bytea, pos int, length int) CASCADE;
CREATE OR REPLACE FUNCTION DataAt(index int, buffer bytea, pos int, len int)
RETURNS bytea
AS $$
BEGIN
	IF($4=0) THEN
		RETURN SUBSTRING($2, GREATEST(0, $3-$1)+1);
	ELSE
		RETURN SUBSTRING($2, GREATEST(0, $3-$1)+1, Overlap($1, $1+LENGTH($2), $3, $3+$4));
	END IF;
END;
$$ LANGUAGE PLpgSQL IMMUTABLE;

--DROP FUNCTION NumberAt(buffer bytea, pos int, length int);
CREATE OR REPLACE FUNCTION NumberAt(index int, buffer bytea, pos int, len int, type int, mul int, div int)
RETURNS double precision
AS $$
DECLARE 
	r bigint:=0;
	b bytea:=DataAt($1, $2, $3, $4);
	i int:=0;
	l int:=LENGTH(b);
BEGIN
	WHILE(i<l) LOOP
		r:=r*256+get_byte(b, i);
		i:=i+1;
	END LOOP;
	RETURN SignNumber(r, $5)*$6/$7::double precision;
END;
$$ LANGUAGE PLpgSQL IMMUTABLE;

CREATE OR REPLACE FUNCTION GetLastDataByIndex(did uuid, idx int, len int)
RETURNS bytea
AS $$
	SELECT DataAt(index, data, $2, $3)
	FROM raw_device_data
	WHERE did=$1
	AND index<$2+$3
	AND (index+LENGTH(data))>$2
	ORDER BY did DESC, stamp DESC, index DESC
	LIMIT 1;
$$ LANGUAGE SQL STABLE;

CREATE OR REPLACE FUNCTION public.insert_as_jsonb()
  RETURNS trigger AS
$BODY$
BEGIN
	SET LOCAL client_min_messages=warning;

	create temp table if not exists refs
	(
	index integer NOT NULL,
	length integer NOT NULL,
	type integer NOT NULL,
	mul integer NOT NULL,
	div integer NOT NULL,
	name character varying not null
	) on commit delete rows;

	SET LOCAL client_min_messages=error;
	
	truncate refs;
	
	insert into refs
    (
		select rf.index, rf.length, rf.type, rf.mul, rf.div, rf.name
		from refs_backend rf
		where rf.hardware = NEW.hardware
		and rf.version = NEW.version 
		and rf.index >= NEW.index
		and (rf.index + rf.length) <= (NEW.index + LENGTH(NEW.data))
		and rf.length != 0
		and rf.length != 16
		order by rf.index
    );

	declare row RECORD;
	declare new_obj jsonb;
	declare prev_jsonb jsonb;
	declare curr_label_id integer;

	declare message text;
	declare code text;
	declare context text;
	
    BEGIN
        FOR row IN (select * from refs) LOOP
		
			select id into curr_label_id
			from device_data_jsonb
			where device_id = NEW.did
			and stamp = new.stamp::date
			and name = row.name;

			new_obj := jsonb_build_object(
				'stamp', date_trunc('minute', new.stamp), 
				'drift', new.drift,
				'val', ROUND(NumberAt(NEW.index, NEW.data, row.index, row.length, row.type, row.mul, row.div)::numeric, 2)
			);

			IF curr_label_id is not null 
				THEN
					select data into prev_jsonb from device_data_jsonb where id = curr_label_id;
					
					IF (prev_jsonb->-1->>'val' <> new_obj->>'val') THEN
						update device_data_jsonb set data = (select jsonb_insert(prev_jsonb, '{-1}', new_obj, true)) where id = curr_label_id;
					END IF;					
				ELSE
					
					insert into device_data_jsonb(device_id, stamp, name, data) values (
						NEW.did, 
						new.stamp,
						row.name,
						jsonb_build_array(new_obj)
					);
			END IF;
            curr_label_id := null;
            prev_jsonb := null;
        END LOOP;
	EXCEPTION
	WHEN OTHERS THEN
		GET STACKED DIAGNOSTICS message = MESSAGE_TEXT,
								code = RETURNED_SQLSTATE,
								context = PG_EXCEPTION_CONTEXT;
		insert into trigger_error(stamp, event, message, code, context) values (now(), TG_OP, message, code, context);
	END;
RETURN NULL;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;
ALTER FUNCTION public.insert_as_jsonb()
  OWNER TO postgres;

  
CREATE OR REPLACE FUNCTION public._insertdevicedata(
    "did" uuid,
    "hardware" integer,
    "version" integer,
    "stamp" timestamp without time zone,
    "drift" integer,
    "store_type" integer,
    "index" integer,
    "data" bytea
    )
  RETURNS void AS
$BODY$
  INSERT INTO device_data_insert_view (did, hardware, version, stamp, drift, index, data, store_type)
  VALUES($1, $2, $3, $4, $5, $7, $8, $6);
$BODY$
  LANGUAGE sql VOLATILE
  COST 100;
ALTER FUNCTION public._insertdevicedata("did" uuid,
    "hardware" integer,
    "version" integer,
    "stamp" timestamp without time zone,
    "drift" integer,
    "store_type" integer,
    "index" integer,
    "data" bytea
    )
  OWNER TO postgres;


CREATE TRIGGER ON_INSERT_DEVICE_DATA
  INSTEAD OF INSERT
  ON public.device_data_insert_view
  FOR EACH ROW
  EXECUTE PROCEDURE public.insert_as_jsonb();
  
CREATE TYPE formatteddatapoint AS (stamp timestamp, drift integer, value double precision);


CREATE OR REPLACE FUNCTION public.gethistoricdatabyname(
    did uuid,
    name character varying,
    begin timestamp without time zone,
    "end" timestamp without time zone)
  RETURNS SETOF formatteddatapoint AS
$BODY$

select (t1.element->'stamp')::text::timestamp, 
	(t1.element->'drift')::text::integer as "drift",
	(t1.element->'val')::text::double precision as "data"
from (	
	select jsonb_array_elements(data) as element
	from device_data_jsonb 
	where device_id = $1
	and name = $2
	and stamp >= $3 and stamp <= $4
	order by stamp
) t1
$BODY$
  LANGUAGE sql STABLE
  COST 100
  ROWS 1000;
ALTER FUNCTION public.gethistoricdatabyname(did uuid, name character varying, begin timestamp without time zone, "end" timestamp without time zone)
  OWNER TO postgres;