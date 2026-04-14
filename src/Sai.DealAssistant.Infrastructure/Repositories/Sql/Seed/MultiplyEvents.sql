insert into public."Events"
("DealId", "Date", "Topic", "Pos", "Agenda", "Result",
 "TypeId", "StateId", "ContactPersonId",
 "GlobalId", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy")
select
    "DealId",
    now() - (random() * interval '730 days')              as "Date",
    topics[floor(random()*array_length(topics,1)+1)]      as "Topic",
    (select coalesce(max(e2."Pos"),0) + 1
       from public."Events" e2
      where e2."DealId" = e."DealId")                     as "Pos",
    'Agenda: ' || topics[floor(random()*array_length(topics,1)+1)] as "Agenda",
    'Result: '  || topics[floor(random()*array_length(topics,1)+1)] as "Result",
    "TypeId",
    "StateId",
    "ContactPersonId",
    gen_random_uuid(), now(), 0, now(), 0
from public."Events" e,
LATERAL (
    SELECT ARRAY[
        'Kick-off Meeting','Requirements Review','Design Review','Sprint Planning',
        'Status Update','Demo Session','Risk Review','Steering Committee',
        'Contract Discussion','Go-Live Review','Post-Mortem','Quarterly Review'
    ] AS topics
) t;
