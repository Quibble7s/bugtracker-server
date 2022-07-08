# Create

Creates a new bug.

**URL** : `/api/v{:version}/bug/project/{:projectID}/create`

**Method** : `POST`

**Authorization**: `Bearer Token`

**Data example**

```json
{
  "name": "string",
  "description": "string",
  "priority": "string"
}
```

**Permissions required** : User must have the role of user or admin.

## Success Response

**Code** : `201 CREATED`

**Content Example**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "string",
  "description": "string",
  "priority": "string",
  "createdAt": "2022-07-08T04:15:41.926Z",
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
  "message": "User {:userID} not found.",
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
