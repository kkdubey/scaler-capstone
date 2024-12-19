import { Utilities } from '../services/utilities';

// Notification model
export class Notification {
  public date = new Date();
  public isRead = false;
  public isPinned = false;
  public id = 0;
  public header = '';
  public body = '';

  public static Create(data: object) {
    const n = new Notification();
    Object.assign(n, data);

    if (n.date) {
      n.date = Utilities.parseDate(n.date);
    }

    return n;
  }
}
