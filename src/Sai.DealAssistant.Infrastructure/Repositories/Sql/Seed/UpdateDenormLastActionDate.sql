UPDATE public."Deals"
   SET "DenormLastActionDate" = sel.max_date
  FROM (
    SELECT e."DealId", max(e."Date") AS max_date
      FROM public."Events" e
     GROUP BY e."DealId"
  ) sel
 WHERE "Id" = sel."DealId";
