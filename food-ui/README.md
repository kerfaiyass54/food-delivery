# FoodUi

This project was generated using [Angular CLI](https://github.com/angular/angular-cli) version 21.2.5.

## Development server

To start a local development server, run:

```bash
ng serve
```

Once the server is running, open your browser and navigate to `http://localhost:4200/`. The application will automatically reload whenever you modify any of the source files.

## Code scaffolding

Angular CLI includes powerful code scaffolding tools. To generate a new component, run:

```bash
ng generate component component-name
```

For a complete list of available schematics (such as `components`, `directives`, or `pipes`), run:

```bash
ng generate --help
```

## Building

To build the project run:

```bash
ng build
```

This will compile your project and store the build artifacts in the `dist/` directory. By default, the production build optimizes your application for performance and speed.

## Running unit tests

To execute unit tests with the [Vitest](https://vitest.dev/) test runner, use the following command:

```bash
ng test
```

## Running end-to-end tests

For end-to-end (e2e) testing, run:

```bash
ng e2e
```

Angular CLI does not come with an end-to-end testing framework by default. You can choose one that suits your needs.

## Additional Resources

For more information on using the Angular CLI, including detailed command references, visit the [Angular CLI Overview and Command Reference](https://angular.dev/tools/cli) page.


## Development identity configuration (until authentication is added)

The frontend does not ship with a default owner or chef ID. Configure the development identities locally in `public/config.json` when you need to open those role-specific screens:

```json
{
  "apiBaseUrl": "/api",
  "ownerId": 12,
  "chefId": 34
}
```

Replace the example IDs with existing rows from your local database. Do not commit personal or production identity values. If an ID is omitted or invalid, the corresponding screen displays a configuration error instead of silently using ID 1.

The client flow uses the ID of the client profile explicitly created in the current browser, persisted as a development profile selection in local storage. This is not authentication or authorization; browser storage can be changed by the user. Replace it with a server-validated identity when authentication is implemented.
