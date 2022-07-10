# Log

Gets the requested log.

**URL** : `/api/v{:version}/log/{:logID}`

**Method** : `GET`

**Authorization**: `Bearer Token`

**Permissions required** : User must have the role of user or admin.

## Success Response

**Code** : `200 OK`

**Content Example**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "messages": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "message": "string",
      "date": "2022-07-10T23:35:19.725Z"
    }
  ]
}
```

## Error Response

**Code** : `401 UNAUTHORIZED`

**Code** : `404 NOT FOUND`

**Content Example**

```json
{
  "message": "Log not found.",
  "status": 404
}
```
