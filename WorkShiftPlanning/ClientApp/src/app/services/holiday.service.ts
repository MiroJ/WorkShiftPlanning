// Holiday utility functions for determining workdays and holidays
export class HolidayService {
  
  /**
   * Get list of holidays for a given year
   * Customize this list based on your region's holidays
   */
  static getHolidays(year: number): Date[] {
    const holidays: Date[] = [
      new Date(year, 0, 1),   // January 1
      new Date(year, 0, 2),   // January 2
      new Date(year, 2, 3),   // March 3
      new Date(year, 3, 10),  // April 10
      new Date(year, 3, 13),  // April 13
      new Date(year, 4, 1),   // May 1
      new Date(year, 4, 6),   // May 6
      new Date(year, 4, 24),  // May 24
      new Date(year, 8, 6),   // September 6
      new Date(year, 8, 22),  // September 22
      new Date(year, 11, 24), // December 24
      new Date(year, 11, 25), // December 25
    ];

    // Example: Add first Monday of September
    // holidays.push(this.getFirstMondayOfMonth(year, 8));
    
    // Example: Add last Monday of May
    // holidays.push(this.getLastMondayOfMonth(year, 4));
    
    // Example: Add 4th Thursday of November (Thanksgiving)
    // holidays.push(this.getNthDayOfWeek(year, 10, 4, 4));

    return holidays;
  }

  /**
   * Check if a date is a workday (not weekend or holiday)
   */
  static isWorkday(date: Date): boolean {
    const dayOfWeek = date.getDay();
    
    // Check if weekend (Saturday = 6, Sunday = 0)
    if (dayOfWeek === 0 || dayOfWeek === 6) {
      return false;
    }

    // Check if holiday
    const holidays = this.getHolidays(date.getFullYear());
    const isHoliday = holidays.some(holiday => 
      holiday.getFullYear() === date.getFullYear() &&
      holiday.getMonth() === date.getMonth() &&
      holiday.getDate() === date.getDate()
    );

    return !isHoliday;
  }

  /**
   * Get the first Monday of a specific month
   */
  private static getFirstMondayOfMonth(year: number, month: number): Date {
    let date = new Date(year, month, 1);
    while (date.getDay() !== 1) {  // 1 = Monday
      date.setDate(date.getDate() + 1);
    }
    return date;
  }

  /**
   * Get the last Monday of a specific month
   */
  private static getLastMondayOfMonth(year: number, month: number): Date {
    const lastDay = new Date(year, month + 1, 0); // Last day of month
    while (lastDay.getDay() !== 1) {  // 1 = Monday
      lastDay.setDate(lastDay.getDate() - 1);
    }
    return lastDay;
  }

  /**
   * Get the Nth occurrence of a specific day of week in a month
   * @param year Year
   * @param month Month (0-11)
   * @param dayOfWeek Day of week (0=Sunday, 1=Monday, etc.)
   * @param occurrence Which occurrence (1=first, 2=second, etc.)
   */
  private static getNthDayOfWeek(year: number, month: number, dayOfWeek: number, occurrence: number): Date {
    let count = 0;
    const daysInMonth = new Date(year, month + 1, 0).getDate();
    
    for (let day = 1; day <= daysInMonth; day++) {
      const date = new Date(year, month, day);
      if (date.getDay() === dayOfWeek) {
        count++;
        if (count === occurrence) {
          return date;
        }
      }
    }
    
    // Return first day of month if not found
    return new Date(year, month, 1);
  }
}
