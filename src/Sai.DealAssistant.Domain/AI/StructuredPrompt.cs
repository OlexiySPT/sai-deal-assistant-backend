namespace Sai.DealAssistant.Domain.AI
{
    public class StructuredPrompt
    {
        // SYSTEM: who the model is + rules
        // EXAMPLE:
        // You are a senior Sales manager writing concise, professional cover letters.
        // Avoid generic phrases.Focus on matching values our company brings to Customer requirements and expectations.
        public string System { get; set; } = string.Empty;

        // USER: the actual task
        // EXAMPLE:
        // Write a cover letter for the following job.
        public string User { get; set; } = string.Empty;

        // CONTEXT: data you inject
        // EXAMPLE:
        // REQUIREMENTS:
        // <put requirements here>
        // COMPANY PROFILE
        // <put company profile here>
        public string Context { get; set; } = string.Empty;

        //OUTPUT: strict format
        // EXAMPLE:
        // Return JSON with fields:
        // - summary(3-4 sentences)
        // - matched_competences(array)
        // - cover_letter(string)
        public string Output { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"SYSTEM:\n{System}\nUSER:\n{User}\nCONTEXT:\n{Context}\nOUTPUT:\n{Output}";
        }
    }
}
