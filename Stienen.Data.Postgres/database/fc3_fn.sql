CREATE PR REPLACE FUNCTION on_insert_device_data_trigger()
	RETURN trigger
	LANGUAGE 'plpgsql'
AS $BODY$
BEGIN
	-- if type 3, skip insert, and call trigger to insert as JSONB, insert all other storeTypes
	-- getLastValue should get last from jsonb, else format a backup message's data
	-- make data conversion function work for text type aswell, some int range bound error on length > 16 types
END;
$BODY$
ALTER FUNCTION public.on_insert_device_data_trigger()
    OWNER TO postgres;