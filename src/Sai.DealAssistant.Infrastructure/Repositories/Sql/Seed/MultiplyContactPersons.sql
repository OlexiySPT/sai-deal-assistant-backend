insert into public."ContactPersons"
("FirmId", "Name", "Position", "Phone", "Email", "Description",
 "GlobalId", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy")
select
    "FirmId",
    firstNames[floor(random()*array_length(firstNames,1)+1)] || ' ' ||
    lastNames[floor(random()*array_length(lastNames,1)+1)]  as "Name",
    positions[floor(random()*array_length(positions,1)+1)]  as "Position",
    '+' || (floor(random()*900)+100)::text || '-' ||
           (floor(random()*900)+100)::text || '-' ||
           (floor(random()*9000)+1000)::text              as "Phone",
    lower(
        firstNames[floor(random()*array_length(firstNames,1)+1)] || '.' ||
        lastNames[floor(random()*array_length(lastNames,1)+1)]
    ) || '@example.com'                                   as "Email",
    'Contact at ' || "Name"                               as "Description",
    gen_random_uuid(), now(), 0, now(), 0
from public."ContactPersons",
LATERAL (
    SELECT
        ARRAY['James','Maria','John','Anna','Robert','Linda','Michael','Barbara',
              'William','Patricia','David','Jennifer','Richard','Susan','Joseph','Karen'] AS firstNames,
        ARRAY['Smith','Johnson','Williams','Brown','Jones','Garcia','Miller','Davis',
              'Wilson','Taylor','Anderson','Thomas','Jackson','White','Harris','Martin'] AS lastNames,
        ARRAY['CEO','CFO','CTO','VP Sales','Account Manager','Product Manager',
              'Business Analyst','Director','Consultant','Partner'] AS positions
) t;
