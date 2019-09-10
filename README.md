# ABSA Assessment Application : Phonebook

## Project setup

Frontend - development 
```
npm install
```

### Compiles and hot-reloads for development
```
npm run serve
```

Backend

- Azure Functions
- Azure Queue
- Azure Table Storage

## Introduction

Phonebook application

## Technology used:

- Vue + material - for frontend 
  - Used Material for UI
- Azure Functions
  - Http triggers for 
    - Adding new entry into queue
    - Retrieving saved data
  - Queue trigger to remove from queue and save into Table Storage
- Azure Queue
- Azure TableStorage

### Setup:

- Need Azure queue named - newentryqueue
- Need Table Storage - PhoneBbook

### Design:

This is difficult !! There are so many ways of doing things and questions to ask. And as you implement the design you realise how and where you could have made things different / better - if you had enough time .. 

I decided to go for a more "front-end" solution.
Keeping the data on the "front-end" / device and using the server as a store for the/a main set. BUT, the app must work without a server - but if server is available - server is master

This approach also has it's own set of problems. The most important is - what if the server is down .. etc

front-end 
- Vue + material

back-end 
- Azure Functions - Http triggers, Queue trigger
- Azure Queue - for new entries
- Azure Table Storage - to save master list
             
### Flow:

App starts - gets data from local storage and load

After load - queries server to get new dataset

Replaces current set with set from server (master set)
- unsaved entries and entries not successfully saved to server will be lost
- the way to address this is:
    - have a "queue" on the client
        - add entry to queue
        - let a queue processor process the entries and report on success/failure
        - queue uses local storage 
    - have merge functionality on the front-end
      - as soon as data from server is retrieved, start a merge process
      - merge all OK records, ask for guidance on all conflicts
      - all new entries and updates to go to queue for processing

As person adds entries - entries are added and saved to local storage, ui updated, and then sent to server for saving

Server puts message in a Queue (storage)

Trigger on queue picks up message and save to table storage

Search - all searching is on the front-end using data on device

KNOWN issues and notes:

1. Validation
   1. I left out validation and kept things simple
   2. The only limitations are fields are not allowed to be more that 20 chars
   3. there is no "pattern" or any validation - GIGO
2. Duplicate entries
   1. Duplicates are allowed : BUT the current code / expects unique names as a key - and will throw errors in the console
   2. I don't know what TODO with duplicates .. allow or not allow - so I allowed it 
3. Other
   1. Vue loads weird - things flash etc - wanted to add splash screen
   2. Add a queue processing mechanism on front end as well
   3. Wanted to add a Hub to send messages to front-end
   
I did not implement AppInsights - that would be a must to see the performance of the functions etc.

Service bus with multiple subscribers would be better solution if you go enterprise - if you have a global solution

would have implemented slightly different if I had more time

Have 2 Phone books - 1 for server, 1 for local. when adding entry choose local vs server - save "local" data also to server as a backup setting / option

the server data - stays on server - can be queried - queries must be pageable - meaning:

When client queries server - server get results and caches it - only a page is sent to the client (limited entries)  - when client scrolls / pages / next - newer pages is retrieved as needed from server - minimal data exchange

if the size of the Phonebook is not THAT huge - keep local version of server on client and use that


Many many ways ...

I did not worry too much about naming standards .. 

I did SOME exception handling ..

I did not implement / deploy solution in AZURE or anywhere - everything was done using local emulators - also why I didn't use Service Bus (no emulator)

When deploying 
- NB .. make sure CORS enabled
- security
- check queue & NAME !!
- check table storage and name
- check url's
















