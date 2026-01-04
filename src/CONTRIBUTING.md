# Contributing Guidelines

## Purpose
This document captures workflow preferences and project-level conventions contributors and tooling should follow when requesting or adding files for review, generation, or modification.

## File access convention (NEW)
When asking the assistant to open or fetch files, always provide project-relative paths prefixed with the workspace root absolute path:

C:\_PROJ\Sai.DealAssistant\Sai.DealAssistant.Backend\src\<project-relative-path>

Examples:
- C:\_PROJ\Sai.DealAssistant\Sai.DealAssistant.Backend\src\Sai.DealAssistant.Application\Entities\EventNotes\Dto\EventNoteDto.cs
- C:\_PROJ\Sai.DealAssistant\Sai.DealAssistant.Backend\src\Sai.DealAssistant.Domain\Entities\EventNote.cs

Notes:
- Use backslashes on Windows paths to match local filesystem conventions.
- If you prefer project-relative paths only (no prefix), indicate that explicitly; otherwise the assistant will try the prefixed absolute path first.

## How to present file requests to the assistant
- Preferred: provide the full absolute path using the prefix described above.
- Acceptable: provide the project-relative path (assistant will attempt the prefixed absolute path automatically).
- If the assistant cannot access a file (authorization, missing workspace, or rate-limiting), paste the file contents directly into the chat or provide the exact file `Full Path` value from Visual Studio __Properties__.

## Troubleshooting
- If the assistant reports "file not found", confirm the path is the exact value shown in Visual Studio __Properties__ -> `Full Path`.
- If the assistant reports authorization/rate-limit errors, re-run the request after ensuring the workspace was added to the assistant context or paste the file contents manually.

## Other contributions
- Please follow the repository's coding standards and formatting rules (see .editorconfig when available).
- Include unit tests for behavior changes and handlers where applicable.

## Contact
If you want this preference removed or changed, add a short note in this file describing the new convention.