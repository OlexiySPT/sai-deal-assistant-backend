insert into public."Deals"
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
    "DenormFirmName"
)
select
    (
        SELECT "Id"
        FROM public."Firms"
        ORDER BY random() + d."Id" * 0 -- to prevent random() result caching
        LIMIT 1
    ) as firmId,
    (CURRENT_DATE - (random() * 365 * 5)::int) as startDate,
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
    "DenormFirmName"
from public."Deals" d;
