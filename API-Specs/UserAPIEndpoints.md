# User API Endpoints

## Create Account

**Endpoint**: POST /api/account/create  

**Description**: Creates a new account and sends a verification email.

**Headers**: None  

**Request Body**:

```json
{
  "email": "user@example.com",
  "password": "Password123!",
  "firstName": "John",
  "lastName": "Doe"
}
```

**Responses**:

- **Status: 200 OK**

```json
{
  "message": "Account created successfully, awaiting verification."
}
```

- **Status: 400 Bad Request**

```json
{
  "message": "Account data is required."
}
```

- **Status: 422 Unprocessable Entity**

```json
{
  "message": "Invalid email format."
}

- **Status: 409 Conflict**

```json
{
  "message": "Email is already registered to an account."
}
```

- **Status: 500 Internal Server Error**

```json
{
  "message": "An error occurred while processing your request."
}
```

## Update Account

**Endpoint**: POST /api/account/update

**Description**: Updates an account's details, including email and name.

**Headers**: None

**Request Body**:

```json
{
  "id": 1,
  "role": "User",
  "email": "user@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "createdAt": "2024-11-04 18:14:13.940",
  "verified": true,
  "addresses": [
    {
      "id": 101,
      "accountID": 1,
      "firstLine": "123 High Street",
      "secondLine": "Apartment 4B",
      "thirdLine": null,
      "fourthLine": null,
      "city": "Sheffield",
      "county": "Yorkshire",
      "country": "England",
      "postCode": "DE557JA",
      "isDefault": true
    }
  ]
}
```

**Responses**:

- **Status: 200 OK**

```json
{
  "message": "Account updated successfully."
}
```

- **Status: 400 Bad Request**

```json
{
  "message": "Expecting ProfileManagementDTO from body of the request"
}
```

- **Status: 404 Not Found**

```json
{
  "message": "User not found."
}
```

- **Status: 422 Unprocessable Entity**

```json
{
  "message": "Invalid email format."
}

- **Status: 409 Conflict**

```json
{
  "message": "Email is already registered to an account."
}
```

- **Status: 500 Internal Server Error**

```json
{
  "message": "An error occurred while processing your request."
}
```

## Check If Email Exists

**Endpoint**: GET /api/account/does-email-exist/{email}

**Description**: Checks if an email address is already registered.

**Headers**: None

**Request Body**: None

**Responses**:

- **Status: 200 OK**

```json
{
  "exists": "true"
}
```

or

```json
{
  "exists": "false"
}
```

- **Status: 400 Bad Request**

```json
{
  "message": "No email address specified."
}
```

- **Status: 422 Unprocessable Entity**

```json
{
  "message": "Invalid email format."
}
```

- **Status: 500 Internal Server Error**

```json
{
  "message": "An error occurred while processing your request."
}
```

## Get Profile Management DTO

**Endpoint**: GET /api/account/get-profile-management-dto/{token}

**Description**: Retrieves the profile management details for a given token.

**Headers**: None

**Request Body**: None

**Responses**:

- **Status: 200 OK**

```json
{
  "id": 1,
  "role": "User",
  "email": "user@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "createdAt": "2024-11-04 18:14:13.940",
  "verified": true,
  "addresses": [
    {
      "id": 101,
      "accountID": 1,
      "firstLine": "123 High Street",
      "secondLine": "Apartment 4B",
      "thirdLine": null,
      "fourthLine": null,
      "city": "Sheffield",
      "county": "Yorkshire",
      "country": "England",
      "postCode": "DE557JA",
      "isDefault": true
    }
  ]
}
```

- **Status: 400 Bad Request**

```json
{
  "message": "Token is missing."
}

- **Status: 500 Internal Server Error**

```json
{
  "message": "An error occurred while processing your request."
}
```
