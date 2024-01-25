export default class UserNotificationSettingsDTO {
    userId: number | null;
    therapyNewsEmailNotification: boolean | false;
    replyEmailNotification: boolean | false;
    subscriptionPreferences: number | 0;
}
