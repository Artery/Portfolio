/* 
Das untenstehende Beispiel ist eine SQL-Abfrage auf die OpenGeoDB (http://opengeodb.org), welche verschiedene Daten aller gespeicherten Deutschen Städte anzeigen sollen, inklusive des zugehörigem Bundeslandes.
Die OpenGeoDB hat eine offene, modulare und hierarische Struktur (siehe http://opengeodb.org/wiki/Datenbank), dadurch ist es etwas kniffilig an die Bundesland Information heranzukommen.
Das untenstehende rekursive SQL-Statement ist nicht super komplex, aber es hat mich einige Zeit gekostet eine korrekte Lösung zu finden, welche das gewünschte Ergebnis liefert.

Die Sturktur der OpenGeoDB ist, dass eine Location aus einer ID und einem Type besteht. Eine Location kann eine Stadt, ein Bundesland oder auch ein Ort sein, wie z.B. das Marine-Ehrenmal von Laboe.
Eine Location kann nun modular aus vielen verschiedenen Daten wie z.B. der Einwohnerzahl als Integer-Wert oder einem Namen als Text bestehen.
Die hierarische Struktur der einzelnen Orte wird durch die sogenannten Ebenen/Layer festgelegt, dabei hat jede Layer-Tiefe einen bestimmten Zweck, 
z.B. Layer 100200000 ist ein Staat, Layer 1007000000 dagegen ist eine Ortschaft. Damit ist jede Location "TeilVon" einer anderen Location, aber die Layer sind nicht zwangsläufig durchgängig.
Z.B. nicht jede Ortschaft untersteht einer politischen Gliederung und nicht jeder Landkreis untersteht einem Regierungsbezirk. (http://opengeodb.org/wiki/Types)
Auch können bestimmte Locations mehrfach vertreten sein z.B. die Stadt Berlin ist sowohl eine Ortschaft als auch ein Bundesland.
All diese Besonderheiten gab es zu beachten, beim durchgehen der einzelnen Ebenen, um zu jedem Ort auch sein passendes Bundesland zu finden.
*/

--id ist die ID des Bundeslandes, layer die Ebene des Bundeslandes und originid die ID des ursprünglichen Ortes
with GetUpperLayers(id, layer, originid) as
(
	(
		--Anchor
		--Es wird gleich mit der überliegenden Ebene gestartet, deswegen werden alle Layer zwischen Postleitzahlgebiet und Landkreis selektiert,
		--um sich so schon gleich einen Schritt zu sparen.
		SELECT td_id.text_val, td_layer.text_val, td_id.loc_id
		FROM geodb_textdata AS td_id, geodb_textdata AS td_layer
		WHERE 
			td_layer.loc_id = td_id.text_val
			AND td_id.text_type = 400100000 
			AND td_layer.text_type = 400200000
			AND td_layer.text_val BETWEEN 5 AND 8
	)
	UNION ALL
	(
		--Recursive member
		--Nun wird sich rekursiv immer weiter an der "TeilVon"-Eigenschaft hochgehangelt.
		--Dabei müssen aber auch alle Fälle behandelt werden, bei denen z.B. ein Layer übersprungen wird,
		--weil z.B. ein ursprünglicher Ort gar nicht Teil eines Regierungsbezirks ist
		--Das heißt manche Orte werden früher schon ihr zugehöriges Bundesland erreichen als andere
		SELECT td_teilvon.text_val, td_layer.text_val, upl.originid
		FROM GetUpperLayers upl, geodb_textdata td_teilvon, geodb_textdata td_layer
		WHERE 
			td_teilvon.loc_id = upl.id  
			AND td_teilvon.text_type = 400100000 
			AND td_layer.text_type = 400200000
			AND td_layer.text_val >= 3 
			AND td_layer.loc_id = td_teilvon.text_val
	)
),
--Hier werden nur noch die relevanten Informationen selektiert
GetBundeslandIDName(originid, bland_name, layer) as
(
	SELECT upl.originid, bland_name.text_val, upl.layer
	FROM GetUpperLayers AS upl LEFT JOIN geodb_textdata AS bland_name 
		ON upl.id = bland_name.loc_id
	WHERE upl.layer = 3 AND bland_name.text_type = 500100000
)

--Große gesamt Abfrage
SELECT gtv.loc_id AS LocationId, plz.text_val AS Plz, name.text_val AS Name, 
	typ.text_val AS Typ, telv.text_val AS Vorwahl, einw.int_val AS EinwohnerAnzahl, gbidn.name AS Bundesland
FROM dbo.geodb_textdata AS gtv LEFT OUTER JOIN
	dbo.geodb_textdata AS name ON gtv.loc_id = name.loc_id LEFT OUTER JOIN
	dbo.geodb_textdata AS typ ON gtv.loc_id = typ.loc_id LEFT OUTER JOIN
	dbo.geodb_textdata AS plz ON gtv.loc_id = plz.loc_id LEFT OUTER JOIN
	dbo.geodb_textdata AS telv ON gtv.loc_id = telv.loc_id LEFT OUTER JOIN
	dbo.geodb_intdata AS einw ON gtv.loc_id = einw.loc_id LEFT OUTER JOIN
	dbo.geodb_locations AS loc ON gtv.loc_id = loc.loc_id LEFT OUTER JOIN
	GetBundeslandIDName AS gbidn ON loc.loc_id = gbidn.originid
WHERE (name.text_type = 500100000) AND (plz.text_type = 500300000) AND (typ.text_type = 400300000) 
AND (telv.text_type = 500400000) AND (einw.int_type = 600700000) AND (gtv.text_type = 400100000)
ORDER BY name