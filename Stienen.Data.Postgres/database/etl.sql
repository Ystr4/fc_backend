copy (
	select device_id, stamp, name, data from 
	(
		select device_id, stamp::date, name, 
			jsonb_agg(data) over (partition by device_id, "stamp"::date, "name" order by "stamp"::time) as "data",
			row_number() over (partition by device_id, "stamp"::date, "name" order by "stamp"::time desc) as rn
		from (
			SELECT  vh."Did" as device_id,
			fr."Name" as name,
			st."CStamp" as stamp,
			jsonb_build_object('val', NumberAt(v."Index", v."Data", fr."Index", fr."Length", ft."Type", ft."Mul", ft."Div"), 'stamp', st."CStamp") as data,
			row_number() over (partition by v."Did", fr."Name", date_trunc('hour', v."Stamp") order by v."Stamp" desc, v."Did") as r
		FROM "VersionHists" vh
		JOIN "FE_References" fr ON fr."Hardware" = vh."Hardware" AND fr."Version" = vh."Version"
		JOIN "FE_Types" ft ON ft."Hardware" = fr."Hardware" AND ft."Version" = fr."Version" AND ft."Id" = fr."Type"
		JOIN "Values" v ON v."Did" = vh."Did" AND v."Index" < (fr."Index" + fr."Length") AND v."Stamp" >= vh."Stamp" AND v."Stamp" <= getlastactive(vh."Did", vh."Stamp")
		JOIN "StoreTypes" st ON st."Did" = vh."Did" AND st."Stamp" = v."Stamp"
		WHERE st."StoreType" = 3
		and (v."Index" + length(v."Data")) > fr."Index"
		and fr."Length" != 0
		and fr."Length" <= 8 
		-- and vh."Did" = 'd2ccad38-cef3-4b16-9a85-9e57f99419c8'
		and v."Stamp" > '2018-12-31'
		) t1 
		where t1.r = 1
		group by device_id, stamp, name, data
	) t2 where t2.rn = 1
) to 'C:\Temp\fc215_data_2019' with binary
