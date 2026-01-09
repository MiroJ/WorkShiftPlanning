# Quick Start Guide

## 🚀 How to Run the Application

### Option 1: Production Mode (Recommended for Testing)
This serves the built Angular app through the ASP.NET Core backend:

```bash
# From the repository root
cd WorkShiftPlanning
dotnet run
```

Then open your browser to: **http://localhost:5000**

### Option 2: Development Mode (Hot Reload)
Run the backend and frontend separately for development:

**Terminal 1 - Backend API:**
```bash
cd WorkShiftPlanning
dotnet run --urls="http://localhost:5000"
```

**Terminal 2 - Angular Dev Server:**
```bash
cd WorkShiftPlanning/ClientApp
npm start
```

Then open your browser to: **http://localhost:4200**

## 🐛 Troubleshooting

### I see only a purple background, no content

**Possible causes:**

1. **The application isn't running**
   - Solution: Make sure you've started the backend with `dotnet run`
   - Check that the console shows: `Now listening on: http://localhost:5000`

2. **JavaScript errors in browser console**
   - Press F12 to open Developer Tools
   - Check the Console tab for errors
   - Common errors:
     - Module loading errors: Make sure you're accessing via http://localhost:5000, not opening index.html directly
     - CORS errors: The API must be running on port 5000

3. **Port conflicts**
   - Check if port 5000 is already in use
   - Change the port: `dotnet run --urls="http://localhost:5001"`
   - Update the API URL in `ClientApp/src/environments/environment.ts`

4. **Browser cache issues**
   - Hard refresh: Ctrl+Shift+R (or Cmd+Shift+R on Mac)
   - Clear browser cache
   - Try incognito/private mode

5. **Build not completed**
   - Rebuild Angular: `cd ClientApp && npm run build`
   - Verify files exist in `wwwroot/` directory

### Check if the app is running correctly

1. **Test the API directly:**
   - Go to: http://localhost:5000/swagger
   - You should see the Swagger UI with API documentation

2. **Test the static files:**
   - Go to: http://localhost:5000
   - Open browser Developer Tools (F12)
   - Check the Console tab for any errors
   - Check the Network tab to see if JavaScript files are loading (should be 200 OK)

3. **Test the API endpoint:**
   ```bash
   curl -X POST http://localhost:5000/api/schedule/calculate \
     -H "Content-Type: application/json" \
     -d '{
       "year": 2026,
       "month": 3,
       "totalStaffCount": 12,
       "numberOfShifts": 2,
       "staffPerShift": 1,
       "firstShiftStartHour": 8,
       "regularStartHour": 8,
       "shiftDuration": 12,
       "standardHours": 7.5,
       "maxOnDutyHoursPerMonth": 36,
       "restHoursBefore": 12,
       "restHoursAfter": 24
     }'
   ```

### Common Error Messages

**"Cannot find path" when running npm commands**
- Make sure you're in the correct directory: `WorkShiftPlanning/ClientApp`
- Run: `cd WorkShiftPlanning/ClientApp` first

**"This localhost page can't be found"**
- The backend isn't running. Start it with `dotnet run`

**"Failed to fetch" or CORS errors in console**
- Make sure the backend is running on port 5000
- Check that CORS is properly configured in Program.cs

**"Cannot GET /" error**
- You're trying to access the Angular dev server (port 4200) without running `npm start`
- Either run `npm start` in ClientApp directory OR use port 5000 to access the built app

## 📝 Development Workflow

### Making Changes to Angular Code

1. Edit files in `WorkShiftPlanning/ClientApp/src/`
2. Rebuild: `cd ClientApp && npm run build`
3. Restart the .NET app or just refresh the browser

### Making Changes to C# Code

1. Edit files in `WorkShiftPlanning/`
2. The app will automatically rebuild (hot reload)
3. Refresh the browser

### Best Practice for Development

Use two terminals:
- Terminal 1: `cd WorkShiftPlanning && dotnet watch run` (auto-rebuilds on C# changes)
- Terminal 2: `cd WorkShiftPlanning/ClientApp && npm start` (hot reload for Angular)
- Access: http://localhost:4200 (proxies API calls to port 5000)

## ✅ Verification Steps

Run these commands to verify your setup:

```bash
# 1. Check .NET SDK
dotnet --version
# Should show: 10.x.x

# 2. Check Node.js
node --version
# Should show: v18.x.x or higher

# 3. Check npm
npm --version
# Should show: 9.x.x or higher

# 4. Build the project
cd WorkShiftPlanning
dotnet build
# Should show: Build succeeded

# 5. Build Angular
cd ClientApp
npm install
npm run build
# Should complete without errors

# 6. Run the app
cd ..
dotnet run
# Should show: Now listening on: http://localhost:5000

# 7. Open browser
# Navigate to: http://localhost:5000
# You should see the input form
```

## 🎯 Expected Behavior

When everything is working correctly:

1. Navigate to http://localhost:5000
2. You should see a form with a purple/gradient background
3. The form title should be "Work Shift Planning System"
4. You should see input fields for year, month, staff count, etc.
5. When you click "Calculate Schedule", it should show results

## 🆘 Still Having Issues?

1. Check the browser console (F12) for JavaScript errors
2. Check the terminal where `dotnet run` is running for C# errors
3. Verify all files were built: `ls wwwroot/` should show .js and .css files
4. Try running in incognito mode to rule out cache issues
5. Verify Node.js and .NET SDK versions match requirements
