# Messages

Gets the requested messages from a log.

**URL** : `/api/v{:version}/log/{:logID}/messages`

**Method** : `GET`

**Authorization**: `Bearer Token`

**Permissions required** : User must have the role of user or admin.

## Success Response

**Code** : `200 OK`

**Content Example**

```json
{
  "count": "number",
  "messages": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "message": "string",
      "date": "2022-07-10T23:36:20.317Z"
    }
  ]
}
```

## Error Response

**Code** : `401 UNAUTHORIZED`
