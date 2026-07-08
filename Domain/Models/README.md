# Domain Models

This folder contains the strongly typed contracts used to map local billing and
automation JSON documents into memory.

Current concrete model areas:

- `Faturamento/`: billing configuration contracts
- `Automation/`: automation recipe contracts

The models remain behavior-light and do not implement invoice rules or browser
automation execution.
