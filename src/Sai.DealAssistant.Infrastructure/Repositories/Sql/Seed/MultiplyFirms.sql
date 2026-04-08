insert into public."Firms"
("Name", "Country", "Description", "GlobalId", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy")
select
    --Name
    prefixes[floor(random()*array_length(prefixes,1)+1)] || ' ' ||
    cores[floor(random()*array_length(cores,1)+1)] || ' ' ||
    suffixes[floor(random()*array_length(suffixes,1)+1)] as nm,
    --Country
    countries[floor(random()*array_length(countries,1)+1)],
    --Other fields
    'Description: ' || prefixes[floor(random()*array_length(prefixes,1)+1)] || ' '
    ||  cores[floor(random()*array_length(cores,1)+1)] || ' ' || prefixes[floor(random()*array_length(prefixes,1)+1)] || ' ' || cores[floor(random()*array_length(cores,1)+1)],
    "GlobalId",
    "CreatedAt",
    "CreatedBy",
    "UpdatedAt",
    "UpdatedBy"
from public."Firms",
LATERAL (
    SELECT
        ARRAY[
            'Global','Advanced','Prime','Dynamic','NextGen','Blue','Green','Smart','Future','United',
            'National','International','Atlantic','Pacific','Northern','Southern','Eastern','Western',
            'Central','Urban','Rural','Digital','Modern','Innovative','Creative','Strategic','Elite',
            'Alpha','Beta','Omega','Vertex','Summit','Pioneer','Horizon','Vision','Core','Quantum',
            'Nova','Apex','Vertex','Synergy','Fusion','Bridge','Peak','Summit','Vertex','Nexus',
            'Orbit','Pulse'
        ] AS prefixes,
        ARRAY[
            'Solutions','Technologies','Systems','Industries','Concepts','Holdings','Group','Services',
            'Enterprises','Partners','Logistics','Consulting','Development','Software','Hardware',
            'Networks','Communications','Energy','Resources','Finance','Capital','Investments',
            'Trading','Retail','Distribution','Manufacturing','Construction','Engineering',
            'Design','Architecture','Marketing','Media','Entertainment','Healthcare','Pharma',
            'Biotech','Agriculture','Food','Beverage','Transport','Mobility','Security','Analytics',
            'Data','Cloud','Automation','Robotics','AI','Innovation'
        ] AS cores,
        ARRAY['Ltd','Inc','LLC','Corp','GmbH','S.A.','Lda'] AS suffixes,
        ARRAY['Netherlands','UK','USA','Ireland','Japan','Germany','Canada'] AS countries
) t;
