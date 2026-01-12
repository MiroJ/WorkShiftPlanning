# Work Shift Planning System

A comprehensive work shift scheduling application with intelligent staff assignment, rest period validation, and workload optimization.

## ?? Run Modes

This application can run in two modes:

### 1. Frontend-Only Mode (Recommended for Standalone Use)
All calculations run in the browser - no backend server needed!

**Quick Start:**
```bash
# Windows
start-frontend.bat

# Linux/Mac
./start-frontend.sh
```

Or manually:
```bash
cd WorkShiftPlanning/ClientApp
npm install
npm start
```

Then open: **http://localhost:4200**

?? **See [ClientApp/README-FRONTEND-ONLY.md](ClientApp/README-FRONTEND-ONLY.md) for complete documentation**

### 2. Full-Stack Mode (ASP.NET Core + Angular)
Backend API with Angular frontend.

**Quick Start:**
```bash
# Windows
WorkShiftPlanning\start.bat

# Or manually
cd WorkShiftPlanning
dotnenet run
```

Then open: **https://localhost:61203** (or port shown in console)

## ?? Features

- ? **24-Hour Shift Coverage** - Automatic staff assignment for continuous coverage
- ? **Regular Work Scheduling** - Standard workday assignments
- ? **Rest Period Enforcement** - Validates minimum rest between shifts
- ? **Monthly Hour Limits** - Tracks extended hours per staff member
- ? **Holiday Management** - Customizable holiday calendar
- ? **Workload Optimization** - Balanced assignment distribution
- ? **Violation Detection** - Identifies scheduling conflicts
- ? **Multiple Views** - Summary, Daily, Staff, and Violations tabs
- ? **Offline Capable** - Frontend-only mode works without internet

## ??? Technology Stack

- **Frontend:** Angular 19, TypeScript 5.8, RxJS
- **Backend (Optional):** ASP.NET Core 10, C# 14
- **Styling:** CSS3 with responsive design

## ?? Requirements

### Frontend-Only Mode
- Node.js 18.19+ or 20.9+
- npm 9+

### Full-Stack Mode
- Node.js 18.19+ or 20.9+
- .NET 10 SDK
- npm 9+

## ?? Quick Start

**Just want to try it?**

```bash
# Clone and run
git clone https://github.com/MiroJ/WorkShiftPlanning
cd WorkShiftPlanning

# Windows
start-frontend.bat

# Linux/Mac
./start-frontend.sh
```

Visit http://localhost:4200 and start scheduling! ??

## ?? Usage

1. **Configure Parameters:**
   - Year and month
   - Total staff count
   - Number of shifts per day
   - Working hours and limits
   - Rest requirements

2. **Calculate Schedule:**
   - Click "Calculate Schedule"
   - Algorithm assigns staff optimally

3. **Review Results:**
   - **Summary:** Overview statistics
   - **Daily Breakdown:** Day-by-day assignments
   - **Staff Schedules:** Individual staff details
   - **Violations:** Rest period or limit issues

## ?? Contributing

Contributions are welcome! If you have suggestions for improvements or new features, 
please open an issue or submit a pull request.

## ?? License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## ?? Support

- **Frontend-Only Mode:** See [ClientApp/README-FRONTEND-ONLY.md](ClientApp/README-FRONTEND-ONLY.md)
- **Full-Stack Mode:** See [TROUBLESHOOTING.md](TROUBLESHOOTING.md)
