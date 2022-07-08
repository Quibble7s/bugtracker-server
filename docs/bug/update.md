# update

Updates the bug.

**URL** : `/api/v{:version}/bug/{:bugID}/project/{:projectID}`

**Method** : `PUT`

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

**Code** : `204 NO CONTENT`

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
