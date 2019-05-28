import * as StaffHome from './StaffHome';
import * as Foodtruck from './Foodtruck';
import * as AddNewOwnership from './AddNewOwnership';
import * as FoodtruckForm from './FoodtruckForm';
import * as PresenceOrUnavailabilityForm from './PresenceOrUnavailabilityForm';

export default {
    staffHome: StaffHome.reducer,
    foodtruckForStaff: Foodtruck.reducer,
    foodtruckForm: FoodtruckForm.reducer,
    presenceOrUnavailabilityForm: PresenceOrUnavailabilityForm.reducer,
    addNewOwnership: AddNewOwnership.reducer
};