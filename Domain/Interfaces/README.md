# Domain Interfaces

This folder contains abstractions owned by the domain boundary.

Current concrete abstractions:

- `IConfigRepository`: asynchronous contract for loading and saving local JSON
  billing configuration and automation recipe documents
- `INfeAutomationService`: asynchronous contract for executing metadata-driven
  browser automation flows and returning the downloaded PDF path

The interfaces here define boundaries only and do not contain storage-specific
implementation details.
