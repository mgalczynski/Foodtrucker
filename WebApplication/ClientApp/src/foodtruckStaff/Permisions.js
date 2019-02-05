export const OWNER = 'OWNER';
export const ADMIN = 'ADMIN';
export const REPORTER = 'REPORTER';
export const ownershipMap = new Map([
    [OWNER, [OWNER, ADMIN, REPORTER]],
    [ADMIN, [REPORTER]],
    [REPORTER, []],
]);