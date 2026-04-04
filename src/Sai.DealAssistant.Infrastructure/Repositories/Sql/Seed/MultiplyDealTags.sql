insert into public."DealTags"
("DealId", "Tag")
select
    "DealId",
    tags[floor(random()*array_length(tags,1)+1)] as "Tag"
from public."DealTags",
LATERAL (
    SELECT ARRAY[
        'priority','follow-up','vip','strategic','upsell','renewal','at-risk',
        'new-logo','partner','enterprise','smb','inbound','outbound','warm',
        'cold','hot','q1','q2','q3','q4','pilot','poc','expansion'
    ] AS tags
) t;
