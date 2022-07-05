# Login

Authenticates the user.

**URL** : `/api/v{:version}/auth/login`

**Method** : `POST`

## Success Response

**Code** : `200 OK`

**Content Example**

```json
{
  "user": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "userName": "string",
    "email": "string",
    "profilePictureUrl": "string",
    "role": "string",
    "createdAt": "2022-07-05T20:29:34.274Z",
    "editedAt": "2022-07-05T20:29:34.274Z",
    "projects": ["3fa85f64-5717-4562-b3fc-2c963f66afa6"]
  },
  "token": "string"
}
```

## Error Response

**Code** : `401 UNAUTHORIZED`

**Content Example**

```json
{
  "message": "Invalid email or password.",
  "status": 401
}
```
