# Biometric API
API which talks to a local nitgen biometric device, perfect for web app integration.

## Compiling
- Requires eNBioBSP SDK libraries to be installed on the system.
- .NET 7 or higher

## Installing from release
You can download an installer from the releases page on github.

# API map
The prefix is: `https://localhost:9000/apiservice/`  
You can change the port at appsettings.json if you ever need in case of conflict.

#### GET: `capture-hash/`
Activates biometric device to capture your fingerprint, in case it all goes well, returns:  
`200 | OK`
```json
{
    "template": "AAAAAZCXZDSfe34t4f//...",  <------- fingerprint hash
    "success": true
}
```
anything else:  
`400 | BAD REQUEST`
```json
{
    "message": "Error on Capture: {nitgen error code}",
    "success": false
}
```

--------------------------------

#### POST: `match-one-on-one/`
Receives a template and activates the biometric device to compare:  
##### POST REQUEST content:
```json
{
    "template": "AAAAAZCXZDSfe34t4f//..."
}
```
in case the procedure of verification goes well, returns:  
`200 | OK`
```json
{
    "message": "Fingerprint matches / Fingerprint doesnt match",
    "success": true/false
}
```
anything else:  
`400 | BAD REQUEST`
```json
{
    "message": "Timeout / Error on Verify: {nitgen error code}",
    "success": false
}
```

--------------------------------

#### GET: `identification/`
Captures your fingerprint and does an index search (1:N) from the memory database, in case it all goes well:  
`200 | OK`
```json
{
    "message": "Fingerprint match found / Fingerprint match not found",  
    "id": id_number,     <------ returns 0 in case its not found
    "success": true/false
}
```
anything else:  
`400 | BAD REQUEST`
```json
{
    "message": "Error on Capture: {nitgen error code}",
    "success": false
}
```

--------------------------------

#### POST: `load-to-memory/`
Receives an __array__ of templates with ID to load in memory:
##### POST REQUEST content:
```json
[
    {
        "id": id_number,        <------ e.g: 1, 2, 3  or 4235, 654646, 23423
        "template": "AAAAAZCXZDSfe34t4f//..."
    },
    {
        "id": id_number,
        "template": "AAAAAZCXZDSfe3ff454t4f//..."
    },
    ...
]
```
in case the procedure of verification goes well, returns:  
`200 | OK`
```json
{
    "message": "Templates loaded to memory",
    "success": true
}
```
anything else:  
`400 | BAD REQUEST`
```json
{
    "message": "Error on AddFIR: {nitgen error code}",
    "success": false
}
```

------------------------------

#### GET: `delete-all-from-memory/`
Deletes all the data stored in memory for index search usage, in case it all goes well, returns:  
`200 | OK`
```json
{
    "message": "All templates deleted from memory",
    "success": true
}
```
