# Update

Updates the project.

**URL** : `/api/v{:version}/project/{:projectID}`

**Method** : `PUT`

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

**Code** : `204 NO CONTENT`

## Error Response

**Code** : `401 UNAUTHORIZED`

**Code** : `404 NOT FOUND`

**Content Example**

```json
{
  "message": "Not found.",
  "status": 404
}
```
