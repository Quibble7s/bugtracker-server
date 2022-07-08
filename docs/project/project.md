# Project

Gets the project.

**URL** : `/api/v{:version}/project/{:projectID}`

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
      "createdAt": "2022-07-08T03:49:47.808Z",
      "tasks": [
        {
          "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
          "description": "string",
          "state": "string"
        }
      ]
    }
  ],
  "createdAt": "2022-07-08T03:49:47.808Z",
  "editedAt": "2022-07-08T03:49:47.808Z"
}
```

## Error Response

**Code** : `404 NOT FOUND`

**Content Example**

```json
{
  "message": "Not found.",
  "status": 404
}
```
