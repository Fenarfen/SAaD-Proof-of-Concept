# Inventory API Endpoints  

## Media

**Endpoint**: GET /api/Media  
**Description**: Gets al media avalible in all branches.

**Headers**:

- `Accept`: application/json

**Request Body**: empty

**Responses**:

- Status: 200 Success
- Body:

```json
[
  {
    "id": 2,
    "branch": {
      "id": 1,
      "name": "Hallam Library",
      "addressFirstLine": "121 Arundel Street",
      "addressSecondLine": "",
      "city": "Sheffield",
      "postCode": "S12NT",
      "opened": "2024-10-10T00:00:00"
    },
    "title": "Catching Fire (Hunger Games, Book Two)",
    "released": "2009-09-01T00:00:00",
    "genre": "Scifi",
    "author": "Suzanne Collins",
    "type": "book"
  },
  {
    "id": 5,
    "branch": {
      "id": 3,
      "name": "Devonshire Park Library",
      "addressFirstLine": "112 Devonshire Street",
      "addressSecondLine": "",
      "city": "Sheffield",
      "postCode": "S37SF",
      "opened": "2024-09-09T00:00:00"
    },
    "title": "Staying on",
    "released": "1977-01-01T00:00:00",
    "genre": "Historical Fiction",
    "author": "Paul Scott",
    "type": "book"
  }
]
```

- Status: 500 Internal Server Error

## Branch

**Endpoint**:
GET /api/Branch  
**Description**: Gets all branches in the same city as the authorised bearer token supplied.
**Headers**:

- `Authorization`: Bearer [token]
- `Accept`: application/json

**Request Body**: empty

**Responses**:

- Status: 200 Success
- Body:

```json
[
  {
    "id": 1,
    "name": "Hallam Library",
    "addressFirstLine": "121 Arundel Street",
    "addressSecondLine": "",
    "city": "Sheffield",
    "postCode": "S12NT",
    "opened": "2024-10-10T00:00:00"
    },
  {
    "id": 3,
    "name": "Devonshire Park Library",
    "addressFirstLine": "112 Devonshire Street",
    "addressSecondLine": "",
    "city": "Sheffield",
    "postCode": "S37SF",
    "opened": "2024-09-09T00:00:00"
  }
]
```

- Status: 500 Internal Server Error
- Status: 401 Unauthorised

## Inventory

**Endpoint**:
GET /api/Inventory
**Description**: Get all avalible inventory(Media) in the same city as the authorised bearer token supplied.
**Headers**:

- `Authorization`: Bearer [token]
- `Accept`: application/json

**Request Body**: empty

**Responses**:

- Status: 200 Success
- Body:

```json
[
  {
    "id": 2,
    "branch": {
      "id": 1,
      "name": "Hallam Library",
      "addressFirstLine": "121 Arundel Street",
      "addressSecondLine": "",
      "city": "Sheffield",
      "postCode": "S12NT",
      "opened": "2024-10-10T00:00:00"
    },
    "title": "Catching Fire (Hunger Games, Book Two)",
    "released": "2009-09-01T00:00:00",
    "genre": "Scifi",
    "author": "Suzanne Collins",
    "type": "book"
  },
  {
    "id": 5,
    "branch": {
      "id": 3,
      "name": "Devonshire Park Library",
      "addressFirstLine": "112 Devonshire Street",
      "addressSecondLine": "",
      "city": "Sheffield",
      "postCode": "S37SF",
      "opened": "2024-09-09T00:00:00"
    },
    "title": "Staying on",
    "released": "1977-01-01T00:00:00",
    "genre": "Historical Fiction",
    "author": "Paul Scott",
    "type": "book"
  }
]
```

- Status: 500 Internal Server Error
- Status: 401 Unauthorised

**Endpoint**:
POST /api/Inventory/Transfer  
**Description**: Creates the media transfers supplied in the body of the request if authorised.
**Headers**:

- `Authorization`: Bearer [token]
- `Accept`: application/json
- `Content-Type`: application/json

**Request Body**:

```json
[
  {
    "id": 0,
    "media": {
      "id": 1,
      "branch": { ... },
      "title": "string",
      "released": "2024-12-18T10:59:49.735Z",
      "genre": "string",
      "author": "string",
      "type": "string"
    },
    "originBranch": {
      "id": 1,
      "name": "string",
      "addressFirstLine": "string",
      "addressSecondLine": "string",
      "city": "string",
      "postCode": "string",
      "opened": "2024-12-18T10:59:49.735Z"
    },
    "destinationBranch": {
      "id": 3,
      "name": "string",
      "addressFirstLine": "string",
      "addressSecondLine": "string",
      "city": "string",
      "postCode": "string",
      "opened": "2024-12-18T10:59:49.735Z"
    },
    "accountID": 0,
    "approved": null,
    "created": "2024-12-18T10:59:49.735Z",
    "completed": null
  }
]
```

**Responses**:

- Status: 200 Success
- Body:

```json
[
  {
    "id": 5,
    "media": {
      "id": 1,
      "branch": { ... },
      "title": "string",
      "released": "2024-12-18T10:59:49.735Z",
      "genre": "string",
      "author": "string",
      "type": "string"
    },
    "originBranch": {
      "id": 1,
      "name": "string",
      "addressFirstLine": "string",
      "addressSecondLine": "string",
      "city": "string",
      "postCode": "string",
      "opened": "2024-12-18T10:59:49.735Z"
    },
    "destinationBranch": {
      "id": 3,
      "name": "string",
      "addressFirstLine": "string",
      "addressSecondLine": "string",
      "city": "string",
      "postCode": "string",
      "opened": "2024-12-18T10:59:49.735Z"
    },
    "accountID": 0,
    "approved": null,
    "created": "2024-12-18T10:59:49.735Z",
    "completed": null
  }
]
```

- Status: 500 Internal Server Error
- Status: 401 Unauthorised
