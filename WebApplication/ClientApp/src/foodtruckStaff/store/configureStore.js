import * as StaffHome from './StaffHome';
import * as Foodtruck from './Foodtruck';
import * as AddNewOwnership from './AddNewOwnership';

export default {
    staffHome: StaffHome.reducer,
    foodtruckForStaff: Foodtruck.reducer,
    addNewOwnership: AddNewOwnership.reducer
};