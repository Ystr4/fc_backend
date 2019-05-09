-- Function: public.insert_as_jsonb()

-- DROP FUNCTION public.insert_as_jsonb();

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
		div integer NOT NULL
	) on commit delete rows;

	SET LOCAL client_min_messages=error;
	
	truncate refs;
	
	insert into refs
    (
		select index, length, type, mul, div
		from refs_backend rf
		where hardware = NEW.hardware
		and version = NEW.version 
		and index >= NEW.index
		and (index + length) <= (NEW.index + LENGTH(NEW.data))
		and length != 0
		and length != 16
		order by index
    );

	declare row RECORD;
	declare new_obj jsonb;
	declare prev_jsonb jsonb;
	declare curr_label_id integer;
	declare last_data bytea;
	declare new_data bytea;

	declare message text;
	declare code text;
	declare context text;
	
    BEGIN
        FOR row IN (select * from refs) LOOP
		
			select id into curr_label_id
			from device_data_jsonb
			where device_id = NEW.did
			and stamp = new.stamp::date
			and index = row.index;

			new_obj := jsonb_build_object(
				'stamp', date_trunc('minute', new.stamp), 
				'drift', new.drift,
				'value', ROUND(NumberAt(NEW.index, NEW.data, row.index, row.length, row.type, row.mul, row.div)::numeric, 2)
			);

			IF curr_label_id is not null 
				THEN
					select data into prev_jsonb from device_data_jsonb where id = curr_label_id;
					
					IF prev_jsonb is not null and (prev_jsonb->-1->>'value' <> new_obj->>'value') THEN
						update device_data_jsonb set data = (select jsonb_insert(prev_jsonb, '{-1}', new_obj, true)) where id = curr_label_id;
					END IF;					
				ELSE
					last_data := getlastdatabyindex(new.did, row.index, row.length);
					new_data := dataat(new.index, new.data, row.index, row.length);
					
					IF new_data is not null && (last_data is null OR last_data <> new_data)
						THEN
						insert into device_data_jsonb(device_id, index, stamp, data) values (
							new.did, 
							row.index,
							new.stamp,
							jsonb_build_array(new_obj)
						);
					END IF;
			END IF;
            curr_label_id := null;
            prev_jsonb := null;
			last_data := null;
			new_data := null;
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
