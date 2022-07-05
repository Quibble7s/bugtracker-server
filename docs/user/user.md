# User

Gets the user.

**URL** : `/api/v{:version}/user/{:userID}`

**Method** : `GET`

**Authorization**: `Bearer Token`

**Permissions required** : User must have the role of user or admin.

## Success Response

**Code** : `200 OK`

**Content Example**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "userName": "string",
  "email": "string",
  "profilePictureUrl": "string",
  "role": "string",
  "createdAt": "2022-07-05T19:41:35.214Z",
  "editedAt": "2022-07-05T19:41:35.214Z",
  "projects": ["3fa85f64-5717-4562-b3fc-2c963f66afa6"]
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
