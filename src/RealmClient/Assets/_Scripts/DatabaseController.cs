using UnityEngine;
using PocketBaseSdk;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace Realm
{
    public class DatabaseController : MonoBehaviour
    {
        public static PocketBase pb;

        private void Start()
        {
            pb = new PocketBase("https://pocketbase.midnightstudio.me", "en-US", AsyncAuthStore.PlayerPrefs);
        }

        // public static void Logout()
        // {
        //     pb.AuthStore.Clear();
        //     SceneManager.LoadScene(0);
        // }

        //==========================Authentication==========================

        public static bool IsLoggedIn()
        {
            return pb.AuthStore != null && pb.AuthStore.IsValid();
        }

        public static bool IsValidUser()
        {
            if (!IsLoggedIn())
                return false;
            if (pb.AuthStore.Model["type"].ToString() == "organization")
            {
                if (pb.AuthStore.Model["verifiedAsOrg"].ToString() == "False")
                    return false;
            }
            return true;
        }

        public static void Logout()
        {
            pb.AuthStore.Clear();
        }

        public static async Task<RecordAuth> AuthenticateUser(string email, string password)
        {
            var userData = await pb.Collection("users").AuthWithPassword(email, password);
            Debug.Log("REALM: User " + userData.Record.Email);
            return userData;
        }

        public static async Task<bool> CreateNewUser(
            string name,
            string email,
            string password,
            string type,
            string organizationId = null
        )
        {
            UserCreateDTO data = new()
            {
                Name = name,
                Email = email,
                Password = password,
                PasswordConfirm = password,
                Type = type,
                OrganizationId = organizationId
            };

            try
            {
                var record = await pb.Collection("users").Create(data);
                if (type != "organization")
                    await AuthenticateUser(email, password);
            }
            catch (Exception e)
            {
                Debug.LogError(e.InnerException.ToString());
                return false;
            }

            return true;
        }

        public static string GetCurrentUserOrganizationId()
        {
            return pb.AuthStore.Model["organizationId"].ToString();
        }

        //============================AR Objects============================

        public static async Task<List<AnchorDTO>> GetAllARObjects()
        {
            var rawAnchors = await pb.Collection("ar_objects").GetFullList();
            List<AnchorDTO> anchors = new();
            foreach (RecordModel anchor in rawAnchors)
            {
                anchors.Add(AnchorDTO.FromRecord(anchor));
            }
            return anchors;
        }

        public static async Task<AnchorDTO> GetARObjectFromAnchorId(string anchorId)
        {
            var anchors = await GetAllARObjects();
            foreach (AnchorDTO anchorDTO in anchors)
            {
                if (anchorDTO.AnchorId == anchorId)
                    return anchorDTO;
            }
            return new AnchorDTO();
        }

        //==============================Tours===============================

        public static async Task<bool> CreateTour(
            string name,
            string description,
            double startLatitude,
            double startLongitude
        )
        {
            TourDTO data = new()
            {
                Name = name,
                Description = description,
                OrganizationId = GetCurrentUserOrganizationId(),
                StartLatitude = startLatitude,
                StartLongitude = startLongitude
            };

            try
            {
                var record = await pb.Collection("tours").Create(data);
            }
            catch (Exception e)
            {
                Debug.LogError(e.InnerException.ToString());
                return false;
            }

            return true;
        }

        public static async Task<List<TourDTO>> GetAllTours()
        {
            var rawTours = await pb.Collection("tours").GetFullList();
            List<TourDTO> tours = new();
            foreach (RecordModel tour in rawTours)
            {
                tours.Add(TourDTO.FromRecord(tour));
            }
            return tours;
        }

        public static async Task<TourDTO> GetTourFromId(string tourId)
        {
            var tours = await GetAllTours();
            foreach (TourDTO tourDTO in tours)
            {
                if (tourDTO.Id == tourId)
                    return tourDTO;
            }
            return new TourDTO();
        }

        public static async Task<bool> UpdateTourFromId(
            string tourId,
            string name,
            string description,
            double startLatitude,
            double startLongitude
        )
        {
            try
            {
                Debug.Log(tourId);
                var record = await pb.Collection("tours").Update(tourId,
                body: new TourDTO()
                {
                    Name = name,
                    Description = description,
                    OrganizationId = GetCurrentUserOrganizationId(),
                    StartLatitude = startLatitude,
                    StartLongitude = startLongitude
                });
            }
            catch (Exception e)
            {
                Debug.LogError(e.InnerException.ToString());
                return false;
            }

            return true;
        }

        //==========================Organizations===========================

        public static async Task<List<OrganizationDTO>> GetAllOrganizations()
        {
            var rawOrganizations = await pb.Collection("organizations").GetFullList();
            List<OrganizationDTO> organizations = new();
            foreach (RecordModel organization in rawOrganizations)
            {
                organizations.Add(OrganizationDTO.FromRecord(organization));
            }
            return organizations;
        }

        public static async Task<OrganizationDTO> GetOrganizationFromId(string organizationId)
        {
            var organizations = await GetAllOrganizations();
            foreach (OrganizationDTO organizationDTO in organizations)
            {
                if (organizationDTO.Id == organizationId)
                    return organizationDTO;
            }
            return new OrganizationDTO();
        }
    }
}