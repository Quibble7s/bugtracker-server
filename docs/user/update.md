# Update

Updates the current user.

**URL** : `/api/v{:version}/user/update`

**Method** : `PUT`

**Authorization**: `Bearer Token`

**Data example**

```json
{
  "userName": "string",
  "email": "string",
  "password": "string",
  "profilePictureUrl": "string"
}
```

**Permissions required** : User must have the role of user or admin.

## Success Response

**Code** : `201 NO CONTENT`

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
