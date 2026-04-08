select 'Firms', count (*) from public."Firms"
union all
select 'Deals', count (*) from public."Deals"
union all
select 'ContactPersons', count (*) from public."ContactPersons"
union all
select 'Events', count (*) from public."Events"
union all
select 'EventNotes', count (*) from public."EventNotes";


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
	||  cores[floor(random()*array_length(cores,1)+1)] || ' ' || prefixes[floor(random()*array_length(prefixes,1)+1)] || ' ' || cores[floor(random()*array_length(cores,1)+1)] , 
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
analyze public."Firms"

SELECT
    prefixes[floor(random()*array_length(prefixes,1)+1)] || ' ' ||
    cores[floor(random()*array_length(cores,1)+1)] || ' ' ||
    suffixes[floor(random()*array_length(suffixes,1)+1)] AS company_name
FROM generate_series(1, 10),
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
) t;

insert into	public."Deals"
(
	"FirmId",
	"StartDate",
	"Name",
	"Description",
	"InitialLetter",
	"Url",
	"AiSearchInfo",
	"AiBriefDescription",
	"Industry",
	"Status",
	"TypeId",
	"StateId",
	"ProposalAmount",
	"MinClientAmount",
	"MaxClientAmount",
	"CurrencyCode",
	"ExchangeRateToEur",
	"AmountTypeId",
	"DenormLastActionDate",
	"GlobalId",
	"CreatedAt",
	"CreatedBy",
	"UpdatedAt",
	"UpdatedBy",
	"DenormFirmName")			
select
	(
		SELECT "Id"
		FROM public."Firms"   
		ORDER BY random() + d."Id" * 0 -- to prevent random() result caching
		LIMIT 1
	) as firmId,
	(CURRENT_DATE - (random() * 365 * 5)::int) as startDate,
	(
		SELECT "Name"
		FROM public."Firms"   
		ORDER BY random() + d."Id" * 0 -- to prevent random() result caching
		LIMIT 1
	) as name,
	(
		SELECT "Description"
		FROM public."Firms"   
		ORDER BY random() + d."Id" * 0 -- to prevent random() result caching
		LIMIT 1
	) as description,
	"InitialLetter",
	"Url",
	"AiSearchInfo",
	"AiBriefDescription",
	"Industry",
	"Status",
	"TypeId",
	"StateId",
	"ProposalAmount",
	"MinClientAmount",
	"MaxClientAmount",
	"CurrencyCode",
	"ExchangeRateToEur",
	"AmountTypeId",
	"DenormLastActionDate",
	"GlobalId",
	"CreatedAt",
	"CreatedBy",
	"UpdatedAt",
	"UpdatedBy",
	"DenormFirmName"
from public."Deals" d;

update public."Deals"
set "DenormFirmName" = '-';

update public."Deals" d
set d."DenormFirmName" = f."Name"
from public."Firms" f
where f."Id" = "FirmId" ;

select count(distinct "DenormFirmName")
from public."Deals" d 
where d."DenormFirmName" = '-';


select count(*)
from public."Deals" d 
--where d."DenormLastActionDate" is not null;

update public."Deals"
set "DenormLastActionDate" = max_date
from (
	select e."DealId" , max(e."Date" ) as max_date
	from public."Events" e 
	group by e."DealId" 
) sel
where "Id" = sel."DealId"  ;

update public."Deals"
set "DenormLastActionDate" = null;

select count (distinct("DealId")) from public."Events"
