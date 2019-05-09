CREATE TABLE round (
  id         	SERIAL NOT NULL, 
  did  		    uuid NOT NULL, 
  name       	varchar(255) NOT NULL, 
  startDate 	timespamp NOT NULL, 
  endDate	 	int4 NOT NULL, 
  selected 		bool NOT NULL,
  PRIMARY KEY (id)
);
  
CREATE TABLE filter_option (
  id         	SERIAL NOT NULL, 
  name       	varchar(255) NOT NULL, 
  active 	    bool NOT NULL, 
  endDate	 	int4 NOT NULL, 
  did  		    uuid NOT NULL, 
  PRIMARY KEY (id)
);  

CREATE TABLE default_counter_setting (
  id         	SERIAL NOT NULL, 
  name       	varchar(255) NOT NULL, 
  active 	    bool NOT NULL, 
  PRIMARY KEY (id)
);

CREATE TABLE custom_counter_setting (
  id         	SERIAL NOT NULL, 
  name       	varchar(255) NOT NULL, 
  PRIMARY KEY (id)
);

CREATE TABLE compare_variable (
  id         	SERIAL NOT NULL, 
  name       	varchar(255) NOT NULL, 
  selected 		bool NOT NULL,
  PRIMARY KEY (id)
);

CREATE TABLE user_filter_setting (
  id         	SERIAL NOT NULL, 
  PRIMARY KEY (id)
);

CREATE TABLE studie (
  id         	SERIAL NOT NULL, 
  selected 		bool NOT NULL,
  PRIMARY KEY (id)
);

CREATE TABLE config (
  id         	SERIAL NOT NULL, 
  refId
  hardware 		int NOT NULL, 
  version		int NOT NULL, 
  name       	varchar(255) NOT NULL,
  PRIMARY KEY (id)
);

CREATE TABLE var_ref(
  id         	SERIAL NOT NULL, 
  hardware 		int NOT NULL, 
  version		int NOT NULL, 
  name       	varchar(255) NOT NULL,
  PRIMARY KEY (id)
);

CREATE TABLE public.device_data
(
  id integer NOT NULL DEFAULT nextval('device_data_id_seq'::regclass),
  device_id uuid,
  stamp date,
  name character varying(31),
  data jsonb,
  CONSTRAINT device_data_pkey PRIMARY KEY (id),
  CONSTRAINT device_data_jsonb_device_id_fkey FOREIGN KEY (device_id)
      REFERENCES public."Devices" ("Id") MATCH SIMPLE
      ON UPDATE RESTRICT ON DELETE CASCADE
)
WITH (
  OIDS=FALSE,
  autovacuum_vacuum_threshold=250,
  autovacuum_vacuum_scale_factor=0.0,
  autovacuum_analyze_threshold=500,
  autovacuum_analyze_scale_factor=0.0
);
ALTER TABLE public.device_data
  OWNER TO postgres;

-- Index: public."UX_did_name_stamp"

-- DROP INDEX public."UX_did_name_stamp";

CREATE UNIQUE INDEX "UX_did_name_stamp"
  ON public.device_data
  USING btree
  (device_id, name COLLATE pg_catalog."default", stamp);
