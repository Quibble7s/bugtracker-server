# Register

Registers a new user.

**URL** : `/api/v{:version}/auth/register`

**Method** : `POST`

**Data example**

```json
{
  "userName": "string",
  "email": "user@example.com",
  "password": "string"
}
```

## Success Response

**Code** : `201 CREATED`

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

**Code** : `400 BAD REQUEST`

**Content Example**

```json
{
  "message": "User {:email} already exist.",
  "status": 400
}
```
