# Bug

Gets the bug.

**URL** : `/api/v{:version}/bug/{:bugID}/project/{:projectID}`

**Method** : `GET`

**Authorization**: `Bearer Token`

**Permissions required** : User must have the role of user or admin.

## Success Response

**Code** : `200 OK`

**Content Example**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "string",
  "description": "string",
  "priority": "string",
  "createdAt": "2022-07-08T04:12:01.063Z",
  "tasks": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "description": "string",
      "state": "string"
    }
  ]
}
```

## Error Response

**Code** : `401 UNAUTHORIZED`

**Code** : `404 NOT FOUND`

**Content Example**

```json
{
  "message": "Bug {:bugID} not found.",
  "status": 404
}
```

**Code** : `404 NOT FOUND`

**Content Example**

```json
{
  "message": "Project {:projectID} not found.",
  "status": 404
}
```

**Code** : `404 NOT FOUND`

**Content Example**

```json
{
  "message": "User {:userID} not found.",
  "status": 404
}
```
