# Create

Creates a new task.

**URL** : `/api/v{:version}/task/project/{:projectID}/bug/{:bugID}`

**Method** : `POST`

**Authorization**: `Bearer Token`

**Data example**

```json
{
  "description": "string"
}
```

**Permissions required** : User must have the role of user or admin.

## Success Response

**Code** : `201 CREATED`

**Content Example**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "description": "string",
  "state": "string"
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
