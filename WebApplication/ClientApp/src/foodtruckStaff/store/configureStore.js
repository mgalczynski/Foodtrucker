import * as StaffHome from './StaffHome';
import * as Foodtruck from './Foodtruck';
import * as AddNewOwnership from './AddNewOwnership';
import * as FoodtruckForm from './FoodtruckForm';

export default {
    staffHome: StaffHome.reducer,
    foodtruckForStaff: Foodtruck.reducer,
    foodtruckForm: FoodtruckForm.reducer,
    addNewOwnership: AddNewOwnership.reducer
};