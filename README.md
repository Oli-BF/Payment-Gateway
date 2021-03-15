# Payment-Gateway

## 1.0 - Key Points

* Fully containerized using **Docker** and **Docker Compose**:
  * **Payment Gateway**
  * **SQL Server** - Implements Persistent Storage
  * **Seq** - Implements Persistent Storage
  * **Prometheus**
  * **Grafana** - Implements Persistent Storage
* **OAuth 2** - Authentication using Okta Developer
* **Swagger** - API Documentation
* **Serilog** - Logging 
* **Seq** - Remote logging (uses Serilog)
* **Prometheus** - Metrics and Data Source for Grafana
* **Grafana** - Contains all metrics / graphs in one place
* **SQL Server** - Data Storage
* **PCI Compliance** (as far as possible):
  * **Database Encryption** for all card details
  * **Card Number Masking** for storing in database and retrieval by merchants
  * **CVV Number is never stored**
* **Unit Tests**

## 2.0 - Setup

### 2.1 - Installation

* Clone repository

* As I have authentication implemented (and as such HTTPS is required), a dev cert is required for Docker to run correctly. If you already have one, please change the password and path to your own in the following settings in the docker-compose.yml file.

  ```dockerfile
  ASPNETCORE_Kestrel__Certificates__Default__Password=yourpassword
  ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
  ```

* If there is still an issue relating to this, then please run the following commands, but changing the password to your own. Please then follow the steps above again:

  ```Powe
  dotnet dev-certs https --clean
  dotnet dev-certs https --trust -ep $env:USERPROFILE\.aspnet\https\aspnetapp.pfx -p YOURPASSWORD
  ```

* CD into project root and run the following command:

  ```powershell
  docker-compose up --build
  ```

* The following services should be created and be running 
  * `pg_core` - Port 80/443 - Swagger available at https://localhost/index.html - API at https://localhost/payment/ (see below)
  * `pg_grafana` - Available at http://localhost:3000
  * `pg_prometheus` - Available at http://localhost:9090
  * `pg_sql_server` - Port 1433
  * `pg_seq`- Available at http://localhost:5341
    * Occasionally, you might see the an error message along the lines of “pg_core excited, cannot connect user sa”. Just wait a moment and pg_core will restart automatically and attempt to connect to the database again. This occurs when the pg_sql_server hasn’t started in time for the pg_core to connect the first time round.
    * Raw metrics are also available at  https://localhost/metrics

### 2.2 - Use

* To send GET / POST requests with Postman / Insomnia or any other API tester (Swagger is just for documentation, for now), you’ll need the following information to send requests with OAuth 2. After inputting the below information, please generate a token and use that to communicate with the Payment Gateway.

  ```
  Grant Type:       Client Credentials
  Access Token URL: https://dev-77360362.okta.com/oauth2/ausbofvvhvl0ao0cN5d6/v1/token
  Client ID:        0oabode4y8rBwJAwe5d6
  Client Secret:    MlLZvp-Qmyx6mgMvWn-3e6aIH6aAka6qx1Jj3605
  Scope:            getpayment
  Credentials:      "As Basic Auth Header"
  Audience:         api://pgauthserv (not required on Postman)
  ```

* Example GET Request

  ```json
  Request: 
  	https://localhost/payment/7c9a28af-8216-490b-8f15-5303fd2d942f
  
  Result: 
      {
        "paymentId": "7c9a28af-8216-490b-8f15-5303fd2d942f",
        "currency": "GBP",
        "amount": 350.00,
        "cardNumberMasked": "************3456",
        "expiryDate": "08/21",
        "cardHolder": "MS JANE HAMILTON-SMITH",
        "paymentSuccessful": true,
        "dateCreated": "2021-03-15T04:55:03.5197975"
      }
  ```

* Example POST Request

  ```json
  Request:
      https://localhost/payment/
      {
          "currency":"GBP",
          "amount":350.00,
          "cardNumber":"1234567890123456",
          "expiryDate":"08/21",
          "cvv":456,
          "cardHolder":"MS JANE HAMILTON-SMITH"
      }
  
  Result:
  	{
    		"paymentId": "3e3a1bc6-8a1e-427d-80cd-ace9d70c3507",
    		"paymentSuccessful": false
  	}
  ```

### 2.3 - Grafana Setup

* Navigate to http://localhost:3000/ and enter the username: `admin` and  password: `admin` (changeable in the docker-compose file). It will then ask you to either set a new password or skip to main page.
* On the menu bar to the left hand side hover over `Configuration` (the cog) and click `Data Sources`.
  * Click `Add Data Source`, then click on `Prometheus`. For `URL` enter: `http://localhost:9090` (make sure there aren’t any leading or trailing spaces) and then set `Access` to `Browser`, leave the rest on the settings on their defaults and click `Save & Test` - a green bar should appear confirming the data source is working.
* On the menu bar to the left hand side hover over the `+` and click `Import`
  * On the `Import via grafana.com`  enter the code `10427` (again make sure there aren’t any leading or trailing spaces)  and click `load`, some settings should appear and under `prometheus` click the drop down box and select the data source added above, keep the rest of the settings at default and click `Import`. The dashboard should open with the metrics showing. 
  * For metrics just about the controller (such as Requests Received, Error Rate and Request Durations) please follow the above and enter code `10915` instead.

## 3.0 - Improvements

* There are several improvements I would have like to have made, but given the time constraints, some concessions had to be made:
  * With this implementation I have injected my DbContext into my controller however, abstracting the data layer away from the controller and using models and services classes, rather than entities and contexts, would perhaps have provided a better approach.
  * Implement HTTPS properly, across all services, with Lets Encrypt and perhaps with something similar to Lettuce Encrypt: https://github.com/natemcmaster/LettuceEncrypt.
  * The database encryption keys are currently within the code, and I would have liked to have moved them to a secure store such as Azure Blob / AWS KMS or something similar. The same also goes for the DBConnection string.
  * More thorough and extensive tests.
  * Basic anti-fraud checks.
  * An API Client.
  * CI / CD Tools.
  * Automatic removal of card data from the database after a certain period of time.
