import * as StaffHome from './StaffHome';
import * as Foodtruck from './Foodtruck';
import * as AddNewOwnership from './AddNewOwnership';
import * as FoodtruckForm from './FoodtruckForm';
import * as PresenceForm from './PresenceForm';

export default {
    staffHome: StaffHome.reducer,
    foodtruckForStaff: Foodtruck.reducer,
    foodtruckForm: FoodtruckForm.reducer,
    presenceForm: PresenceForm.reducer,
    addNewOwnership: AddNewOwnership.reducer
};