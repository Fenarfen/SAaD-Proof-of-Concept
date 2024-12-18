# Auth API Endpoints

## Send Email Verification Code

**Endpoint**: POST /api/auth/send-email-verification-code/{accountID}  
**Description**: Sends a 6-digit email verification code to the specified account's email address.

**Headers**:  

- `Authorization`: Bearer <user_api_key>  

**Request Body**: None  

**Responses**:  

- **Status: 200 OK**

```json
{
    "message": "success"
}
```

- **Status: 400 Bad Request**

```json
{
    "message": "User does not exist."
}
```

or

```json
{
  "message": "Account is already verified"
}
```

- **Status: 500 Internal Server Error**

```json
{
  "message": "An error occurred while processing your request."
}
```

## Verify Account

**Endpoint**: POST /api/auth/verify-account
**Description**: Verifies the account using the provided email and code.

**Headers**: None

**Request Body**:

```json
{
  "email": "user@example.com",
  "code": "123456"
}
```

**Responses**:

- **Status: 200 OK**

```json
{
  "message": "success"
}
```

- **Status: 400 Bad Request**

```json
{
  "message": "User does not exist."
}
```

or

```json
{
  "message": "Code is incorrect."
}
```

- **Status: 500 Internal Server Error**

```json
{
  "message": "An error occurred while processing your request."
}
```

## Login

**Endpoint**: POST /api/auth/login

**Description**: Authenticates a user based off their email and password, and returns a token and role.

**Headers**: None

**Request Body**:

```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

**Responses**:

- **Status: 200 OK**

```json
{
  "token": "generated_token",
  "role": "User"
}
```

- **Status: 400 Bad Request**

```json
{
  "message": "Incorrect log in details"
}
```

- **Status: 500 Internal Server Error**

```json
{
  "message": "An error occurred while processing your request."
}
```

## Verify Token

**Endpoint**: POST /api/auth/verify-token

**Description**: Verifies if a token is valid.

**Headers**: None

**Request Body**:

```json
{
  "token": "your_token"
}
```

**Responses**:

- **Status: 200 OK**

```json
{
  "message": "Token is valid"
}
```

- **Status: 400 Bad Request**

```json
{
  "message": "Token is invalid"
}
```

- **Status: 500 Internal Server Error**

```json
{
  "message": "An error occurred while processing your request."
}
```

## Get City and Role by Token

**Endpoint**: POST /api/auth/get-city-role-by-token/{userApiToken}

**Description**: Retrieves the city and role of a user using their token.

**Headers**:

- `Authorization`: Bearer <user_api_key>  

**Request Body**: None

**Responses**:

- **Status: 200 OK**

```json
{
  "role": "User",
  "city": "Sheffield"
}
```

- **Status: 404 Not Found**

```json
{
  "message": "User API Token is invalid or not found."
}
```

- **Status: 500 Internal Server Error**

```json

{
  "message": "An error occurred while processing your request."
}
```
