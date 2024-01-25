import UserDto from "../../models/user/UserDto";

const LocalStorageKeys = {
  AUTH_TOKEN: "AUTHTOKEN",
  CURRENT_USER: "CURRENTUSER",
    IMPERSONATING: "IMPERSONATING",
  SAVED_MFA: "SAVEDMFA"
};

const store = {
  saveToken(token: string): void {
    localStorage.setItem(LocalStorageKeys.AUTH_TOKEN, token);
  },
  clearToken(): void {
    localStorage.removeItem(LocalStorageKeys.AUTH_TOKEN);
  },
  getToken(): string {
    var tokenJson = localStorage.getItem(LocalStorageKeys.AUTH_TOKEN);

    if (!tokenJson) return null;

    return tokenJson;
  },
  saveCurrentUser(user: UserDto): void {
    localStorage.setItem(LocalStorageKeys.CURRENT_USER, JSON.stringify(user));
  },
  clearCurrentUser(): void {
    localStorage.removeItem(LocalStorageKeys.CURRENT_USER);
  },
  getCurrentUser(): UserDto {
    var userJson = localStorage.getItem(LocalStorageKeys.CURRENT_USER);

    if (!userJson) return null;

    return JSON.parse(userJson) as UserDto;
  },
  isUserLoggedIn(): boolean {
    return !(!this.getToken() || !this.getCurrentUser());
  },
    setImpersonating(user: UserDto): void {
        localStorage.setItem(LocalStorageKeys.IMPERSONATING, JSON.stringify(user));
    },
    stopImpersonating(): void {
        localStorage.removeItem(LocalStorageKeys.IMPERSONATING);
    },
    getImpersonatedUser(): UserDto {
        const impersonatedUserJson = localStorage.getItem(LocalStorageKeys.IMPERSONATING);

        if (!impersonatedUserJson) return null;

        return JSON.parse(impersonatedUserJson) as UserDto;
    },
    isImpersonating(): boolean {
        return this.getImpersonatedUser() !== null;
    },
    getImpersonatedOrCurrentUser(): UserDto {
        return this.getImpersonatedUser() ?? this.getCurrentUser();
    },
    saveMfa(mfa: string): void {
        localStorage.setItem(LocalStorageKeys.SAVED_MFA, mfa);
    },
    clearMfa(): void {
        localStorage.removeItem(LocalStorageKeys.SAVED_MFA);
    },
    getMfa(): string {
        var tokenJson = localStorage.getItem(LocalStorageKeys.SAVED_MFA);

        if (!tokenJson) return null;

        return tokenJson;
    },
    hasSavedMFA(): boolean {
        return (this.getMfa() != null);
    },
};

export default store;
