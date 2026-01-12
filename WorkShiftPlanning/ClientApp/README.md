# Work Shift Planning System - Frontend Only Mode

## Overview

The application now runs **entirely in the browser** with all schedule calculation logic implemented in TypeScript/Angular. No backend server is required!

## Quick Start

### Option 1: Serve with Angular CLI (Development)

```bash
cd WorkShiftPlanning/ClientApp
npm install
npm start
```

Then open your browser to: **http://localhost:4200**

### Option 2: Build and Serve Statically (Production)

```bash
cd WorkShiftPlanning/ClientApp
npm install
npm run build
```

Then serve the `WorkShiftPlanning/wwwroot` directory with any static web server:

**Using Python:**
```bash
cd ../wwwroot
python -m http.server 8080
```

**Using Node.js http-server:**
```bash
npm install -g http-server
cd ../wwwroot
http-server -p 8080
```

**Using live-server:**
```bash
npm install -g live-server
cd ../wwwroot
live-server --port=8080
```

Then open: **http://localhost:8080**

## ?? Architecture

### Frontend-Only Components

1. **HolidayService** (`holiday.service.ts`)
   - Determines workdays and holidays
   - Customizable holiday calendar

2. **ScheduleCalculatorService** (`schedule-calculator.service.ts`)
   - Core scheduling algorithm
   - Staff assignment logic
   - Rest period validation
   - Converted from C# to TypeScript

3. **ScheduleService** (`schedule.service.ts`)
   - Wrapper service
   - Now calls local calculator instead of API

### Data Flow

```
Input Component
    ?
ScheduleService.calculateSchedule()
    ?
ScheduleCalculatorService.calculateSchedule()
    ?
HolidayService.isWorkday()
    ?
ScheduleResult (in memory)
    ?
Results Component
```

## ?? Features

All features work identically to the backend version:

- ? 24-hour shift coverage scheduling
- ? Regular workday assignments
- ? Rest period enforcement
- ? Monthly extended hours limits
- ? Holiday and weekend handling
- ? Workload distribution optimization
- ? Violation detection and reporting

## ?? Configuration

### Customizing Holidays

Edit `WorkShiftPlanning/ClientApp/src/app/services/holiday.service.ts`:

```typescript
static getHolidays(year: number): Date[] {
  const holidays: Date[] = [
    new Date(year, 0, 1),   // January 1
    new Date(year, 11, 25), // December 25
    // Add your holidays here
  ];
  return holidays;
}
```

### Build Configuration

The Angular app is configured in `angular.json`:
- Output directory: `../wwwroot`
- No server-side rendering
- Optimized for production builds

## ?? Development

### File Structure

```
ClientApp/
??? src/
?   ??? app/
?   ?   ??? components/
?   ?   ?   ??? schedule-input/      # Input form
?   ?   ?   ??? schedule-results/    # Results display
?   ?   ??? models/
?   ?   ?   ??? schedule.models.ts   # TypeScript interfaces
?   ?   ??? services/
?   ?   ?   ??? holiday.service.ts           # Holiday logic
?   ?   ?   ??? schedule-calculator.service.ts  # Core algorithm
?   ?   ?   ??? schedule.service.ts          # Main service
?   ?   ?   ??? schedule-data.service.ts     # State management
?   ?   ??? app.module.ts
?   ??? index.html
??? package.json
```

### Making Changes

1. Edit TypeScript files in `ClientApp/src/`
2. Angular will auto-reload (if using `npm start`)
3. Or rebuild: `npm run build`

## ?? Performance

**Benefits of Frontend-Only Mode:**
- ? Instant calculations (no network latency)
- ?? Works offline after initial load
- ?? Deploy to any static host (GitHub Pages, Netlify, Vercel, etc.)
- ?? No server costs
- ?? Data never leaves the browser

**Limitations:**
- Initial bundle size: ~360 KB (gzipped: ~93 KB)
- Calculation runs in browser (may be slower for very large schedules)
- No server-side data persistence (use browser storage if needed)

## ?? Deployment Options

### GitHub Pages

```bash
# Build the app
cd ClientApp
npm run build

# Deploy wwwroot folder to GitHub Pages
# (Use gh-pages branch or GitHub Actions)
```

### Netlify / Vercel

1. Point to `WorkShiftPlanning/wwwroot` directory
2. No build command needed (pre-built)
3. Serve as SPA with fallback to `index.html`

### Azure Static Web Apps / AWS S3

Upload the `wwwroot` folder contents with proper SPA routing configuration.

## ?? Migrating from Backend Version

The frontend-only version is **100% compatible** with the backend version. The same TypeScript interfaces are used, and results are identical.

**What Changed:**
- ? Removed: HTTP calls to `/api/schedule/calculate`
- ? Added: Local `ScheduleCalculatorService`
- ? Added: `HolidayService` for date calculations
- ?? Same: All TypeScript models and interfaces
- ?? Same: UI components and styling

## ?? Troubleshooting

### Build Errors

```bash
# Clear node_modules and reinstall
rm -rf node_modules package-lock.json
npm install
npm run build
```

### Incorrect Calculations

The algorithm is a direct port from C#. If results differ:
1. Check `HolidayService.getHolidays()` configuration
2. Verify input parameters match expected format
3. Compare with backend version for debugging

### Performance Issues

For large schedules (100+ staff, multiple months):
- Consider using Web Workers (future enhancement)
- Break calculation into chunks
- Add progress indicators

## ?? Notes

- The C# backend code is still present but not used
- You can run either version (backend API or frontend-only)
- Both produce identical results
- Choose based on your deployment requirements

---

**Frontend-Only Mode:** Perfect for demos, prototypes, and deployments where server infrastructure isn't available or needed!
