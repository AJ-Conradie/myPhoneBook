# POC / Assessment Application : Phonebook
## Developed on Windows 10, with Visual Studio Code and Visual Studio 2017 as a POC / Assessment

Not in production ready mode - needs some TLC in that regards - like logging and exception handling

## Project setup

Initial - Make folder and get code

```
cd\
md github.com
cd github.com
md aj-conradie
cd aj-conradie
git clone https://github.com/AJ-Conradie/myPhoneBook.git
cd myphonebook
npm install
```

### Frontend
Vue - development 


```
cd\github.com\aj-conradie\myphonebook
npm install
```

### Compiles and hot-reloads for development
```
npm run serve
```

### Make production ready - distribution in `myphonebook\dist` folder
```
npm run build
```

Local will be listening on :  http://localhost:8080/

### Backend

Visual Studio 2017 was used for this

- Azure Functions
- Azure Queue
- Azure Table Storage

```
BEFORE starting local dev
make sure Storage Emulator up and running and all Azure stuff installed
check queues - there must be a 'newentryqueue'
check table storage - there must be a 'PhoneBook'

not sure about case - so please KEEP CASE
```

```
goto folder : github.com\aj-conradie\myPhoneBook\myPhoneBookAPI
open file   : myPhoneBookApi.sln
restore nuget packages
build
run
```
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
- the way to address this is: TODO
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
   2. I don't know what to do with duplicates .. allow or not allow - so I allowed it 
3. Other
   1. Vue loads weird - things flash etc - wanted to add splash screen
   2. Add a queue processing mechanism on front end as well
   3. Wanted to add a Hub to send messages to front-end
   
I did not implement AppInsights - that would be a must to see the performance of the functions etc.


## Other possible way

Would have implemented slightly different if I had more time

Have 2 Phone books - 1 for server, 1 for local. when adding entry it goes to local - local is saved on client as private phonebook - save "local" data also to server as a backup setting / option

The server data stays on server - can be queried - queries must be pageable - meaning:
server entries only be added by admin or when one choses to save entry on server

When client queries server - server get results and caches it - only a page is sent to the client (limited entries)  - when client scrolls / pages / next - newer pages is retrieved as needed from server - minimal data exchange

if the size of the Phonebook is not THAT huge - keep local version of server on client and use that

When client searches - it searches local and global async

Many many ways ...

I did not worry too much about naming standards .. 

I did SOME exception handling ..

I did not implement / deploy solution in AZURE or anywhere - everything was done using local emulators - also why I didn't use Service Bus (no emulator)

When deploying - things to keep in mind
- make sure CORS setup correctly
- security
- check queue & NAME !!
- check table storage and NAME !!
- check url's
















