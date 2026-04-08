UPDATE public."Deals" d
   SET "DenormFirmName" = f."Name"
  FROM public."Firms" f
 WHERE d."FirmId" = f."Id";
