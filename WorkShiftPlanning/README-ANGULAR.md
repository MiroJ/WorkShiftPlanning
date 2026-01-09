# Work Shift Planning System - Angular UI

This project provides a modern Angular-based user interface for the Work Shift Planning System, backed by an ASP.NET Core Web API.

## ?? Features

- **Interactive Input Form**: Configure schedule parameters with real-time validation
- **Comprehensive Results Display**: View schedules in multiple formats:
  - Summary overview with statistics
  - Daily breakdown of all assignments
  - Individual staff schedules
  - Rest period violation tracking
- **Responsive Design**: Works seamlessly on desktop and mobile devices
- **Real-time Calculations**: Instant schedule generation via REST API

## ?? Prerequisites

Before you begin, ensure you have the following installed:

- **Node.js** (v18 or higher) - [Download here](https://nodejs.org/)
- **.NET 10 SDK** - [Download here](https://dotnet.microsoft.com/download)
- **Angular CLI** (v18) - Install via: `npm install -g @angular/cli`

## ?? Getting Started

### 1. Install Node.js Dependencies

Navigate to the Angular app directory and install dependencies:

```bash
cd WorkShiftPlanning/ClientApp
npm install
```

### 2. Run the Application

You have two options to run the application:

#### Option A: Run Both Backend and Frontend Together (Recommended for Development)

From the project root directory:

```bash
cd WorkShiftPlanning
dotnet run
```

This will:
- Start the ASP.NET Core Web API on `http://localhost:5000`
- Automatically launch the Angular dev server on `http://localhost:4200`
- Configure proxy to route API calls from Angular to the backend

Open your browser and navigate to: `http://localhost:4200`

#### Option B: Run Backend and Frontend Separately

**Terminal 1 - Start the Web API:**
```bash
cd WorkShiftPlanning
dotnet run --launch-profile "http"
```

**Terminal 2 - Start the Angular App:**
```bash
cd WorkShiftPlanning/ClientApp
npm start
```

Open your browser and navigate to: `http://localhost:4200`

### 3. Using the Application

1. **Configure Parameters**: On the input screen, set your schedule parameters:
   - Time period (year and month)
   - Staff configuration
   - Shift settings
   - Work hours and rest requirements

2. **Calculate Schedule**: Click "Calculate Schedule" to generate the shift plan

3. **View Results**: Navigate through the tabs to see:
   - **Summary**: Overview statistics and configuration
   - **Daily Breakdown**: Day-by-day schedule details
   - **Staff Schedules**: Individual staff member assignments
   - **Violations**: Any rest period violations (if applicable)

## ??? Project Structure

```
WorkShiftPlanning/
??? Controllers/
?   ??? ScheduleController.cs          # REST API endpoints
??? DTOs/
?   ??? ScheduleDtos.cs                # Data transfer objects
??? Models/
?   ??? *.cs                           # Domain models
??? ClientApp/                         # Angular application
?   ??? src/
?   ?   ??? app/
?   ?   ?   ??? components/
?   ?   ?   ?   ??? schedule-input/   # Input form component
?   ?   ?   ?   ??? schedule-results/ # Results display component
?   ?   ?   ??? models/
?   ?   ?   ?   ??? schedule.models.ts # TypeScript interfaces
?   ?   ?   ??? services/
?   ?   ?   ?   ??? schedule.service.ts # API communication
?   ?   ?   ??? app.module.ts
?   ?   ?   ??? app-routing.module.ts
?   ?   ??? styles.css                 # Global styles
?   ?   ??? index.html
?   ??? angular.json
?   ??? package.json
?   ??? tsconfig.json
??? ProgramAPI.cs                      # Web API startup
??? WorkShiftPlanning.csproj
```

## ?? Configuration

### API Endpoint Configuration

The Angular app is configured to communicate with the backend API:

- **Development**: `http://localhost:5000/api` (via proxy)
- **Production**: `/api` (same origin)

To change the API URL, update:
- `WorkShiftPlanning/ClientApp/src/environments/environment.ts` (development)
- `WorkShiftPlanning/ClientApp/src/environments/environment.prod.ts` (production)

### CORS Configuration

The Web API is configured to allow requests from `http://localhost:4200` during development. Update `ProgramAPI.cs` if you need to add additional origins.

## ?? Building for Production

To create a production build:

```bash
cd WorkShiftPlanning/ClientApp
npm run build
```

This will build the Angular app and output the files to `WorkShiftPlanning/wwwroot/`.

To publish the entire application:

```bash
cd WorkShiftPlanning
dotnet publish -c Release
```

## ??? Development

### Running the Console Application

The original console application is still available:

```bash
cd WorkShiftPlanning
dotnet run --project Program.cs
```

### Angular Development Server

For Angular-only development with hot reload:

```bash
cd WorkShiftPlanning/ClientApp
ng serve
```

### API Testing

The Web API includes Swagger documentation available at:
`http://localhost:5000/swagger`

## ?? API Endpoints

- **POST** `/api/schedule/calculate`
  - Calculates work shift schedule
  - Request body: `ScheduleRequestDto`
  - Response: `ScheduleResultDto`

## ?? Customization

### Styling

- Global styles: `ClientApp/src/styles.css`
- Component styles: Each component has its own `.css` file

### Default Values

Default form values can be modified in:
`ClientApp/src/app/components/schedule-input/schedule-input.component.ts`

## ?? Troubleshooting

### Port Already in Use

If port 4200 or 5000 is already in use:

1. Change the Angular port in `ClientApp/angular.json` (serve > options > port)
2. Change the API port in `ProgramAPI.cs` or via command: `dotnet run --urls="http://localhost:5001"`
3. Update the proxy configuration in `ClientApp/proxy.conf.json`

### Node Modules Issues

If you encounter issues with node modules:

```bash
cd WorkShiftPlanning/ClientApp
rm -rf node_modules package-lock.json
npm install
```

### Build Errors

Ensure you have the correct versions:
```bash
node --version    # Should be v18 or higher
npm --version     # Should be 9 or higher
ng version        # Should be Angular CLI 18
dotnet --version  # Should be .NET 10
```

## ?? License

This project is part of the Work Shift Planning System.

## ?? Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## ?? Support

For issues or questions, please create an issue in the GitHub repository.
