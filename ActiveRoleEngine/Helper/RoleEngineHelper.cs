using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ActiveRoleEngine.Helper
{
    public static class RoleEngineHelper
    {
        public static string GetPermissionId(string permission, string area, string controller, string action)
        {
            /*
             * Ouput:
             * area/controller/action
             * area/controller
             * controller
             * controller/area
             * 
             * */

            if (permission.IsNotNullOrEmpty())
                return permission.TrimSafe();

            area = area.TrimSafe();
            controller = controller.TrimSafe();
            action = action.TrimSafe();

            if (area.IsNotNullOrEmpty())
                area = area + "/";
            if (action.IsNotNullOrEmpty())
                action = "/" + action;

            return $"{area}{controller}{action}";
        }

        #region GetWebEntryAssembly

        // https://jacstech.wordpress.com/2013/09/05/get-the-executing-assembly-of-a-web-app-referencing-a-class-library/
        /// <summary>
        /// Gets the web entry assembly.
        /// </summary>
        /// <returns></returns>
        internal static Assembly GetWebEntryAssembly()
        {
            // todo later
            //if (HttpContext.Current == null ||
            //    HttpContext.Current.ApplicationInstance == null)
            //{
            //    return null;
            //}

            //Type type = HttpContext.Current.ApplicationInstance.GetType();
            //while (type != null && type.Namespace == "ASP")
            //{
            //    type = type.BaseType;
            //}

            //return type?.Assembly;
            return null;
        }

        #endregion GetWebEntryAssembly

        #region AllAreaTypes

        private static IEnumerable<Type> _allAreaTypes;

        /// <summary>
        /// Gets all area types
        /// </summary>
        /// <value>
        /// All area types
        /// </value>
        internal static IEnumerable<Type> AllAreaTypes
        {
            get
            {
                if (_allAreaTypes == null)
                {
                    Assembly assembly = GetWebEntryAssembly();
                    _allAreaTypes = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(AreaRegistration)));
                }

                return _allAreaTypes;
            }
        }

        #endregion AllAreaTypes


        #region GetControllerArea

        private static readonly Dictionary<string, string> _controllerAreaDictionary = new Dictionary<string, string>();

        /// <summary>
        /// Get the area name from controller type
        /// </summary>
        /// <param name="controllerType">Type of the controller.</param>
        /// <returns></returns>
        public static string GetControllerArea(Type controllerType)
        {
            if (controllerType == null)
                throw new ArgumentNullException(nameof(controllerType));

            if (_controllerAreaDictionary.ContainsKey(controllerType.Namespace))
                return _controllerAreaDictionary[controllerType.Namespace];

            foreach (Type areaType in AllAreaTypes)
            {
                if (!controllerType.Namespace.StartsWith(areaType.Namespace)) continue;

                AreaRegistration area = (AreaRegistration)Activator.CreateInstance(areaType);

                _controllerAreaDictionary.Add(controllerType.Namespace, area.AreaName);
                return area.AreaName;
            }

            _controllerAreaDictionary.Add(controllerType.Namespace, string.Empty);
            return string.Empty;
        }

        #endregion GetControllerArea

        #region GetControllerName

        /// <summary>
        /// Get the controller name without 'Controller' suffix
        /// </summary>
        /// <param name="controllerType">Type of the controller.</param>
        /// <returns></returns>
        public static string GetControllerName(Type controllerType)
        {
            return controllerType.Name.TrimEnd("Controller");
        }

        #endregion GetControllerName
    }

}
