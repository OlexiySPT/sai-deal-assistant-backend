insert into public."EventNotes"
("EventId", "Order", "Text")
select
    "EventId",
    (select coalesce(max(n2."Order"),0) + 1
       from public."EventNotes" n2
      where n2."EventId" = n."EventId")                   as "Order",
    notes[floor(random()*array_length(notes,1)+1)]        as "Text"
from public."EventNotes" n,
LATERAL (
    SELECT ARRAY[
        'Follow-up required.','Action items assigned.','Awaiting client feedback.',
        'Next steps defined.','Escalation needed.','Approved by stakeholders.',
        'Blocked — dependency outstanding.','Completed successfully.',
        'Risk identified and logged.','Budget discussed.'
    ] AS notes
) t;
