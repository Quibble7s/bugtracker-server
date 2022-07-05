# Users

Gets all the users.

**URL** : `/api/v{:version}/user`

**Method** : `GET`

**Authorization**: `Bearer Token`

**Permissions required** : User must have the role of admin.

## Success Response

**Code** : `200 OK`

**Content Example**

```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "userName": "string",
    "email": "string",
    "profilePictureUrl": "string",
    "role": "string",
    "createdAt": "2022-07-05T19:38:17.983Z",
    "editedAt": "2022-07-05T19:38:17.983Z",
    "projects": ["3fa85f64-5717-4562-b3fc-2c963f66afa6"]
  }
]
```

## Error Response

**Code** : `401 UNAUTHORIZED`
