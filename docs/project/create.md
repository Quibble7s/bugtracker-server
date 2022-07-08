# Create

Creates a new project.

**URL** : `/api/v{:version}/project/create`

**Method** : `POST`

**Authorization**: `Bearer Token`

**Data example**

```json
{
  "name": "string",
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
  "name": "string",
  "description": "string",
  "members": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "user": {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "userName": "string",
        "profilePictureUrl": "string"
      },
      "role": "string"
    }
  ],
  "bugs": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "string",
      "description": "string",
      "priority": "string",
      "createdAt": "2022-07-08T03:57:59.072Z",
      "tasks": [
        {
          "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
          "description": "string",
          "state": "string"
        }
      ]
    }
  ],
  "createdAt": "2022-07-08T03:57:59.072Z",
  "editedAt": "2022-07-08T03:57:59.072Z"
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
