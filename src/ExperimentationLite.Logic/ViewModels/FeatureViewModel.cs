using System;
using System.ComponentModel.DataAnnotations;

namespace ExperimentationLite.Logic.ViewModels
{
    public class FeatureViewModel : BaseFeatureViewModel
    {
        [Required(AllowEmptyStrings = false,
            ErrorMessage = "A feature ID cannot be an empty string and must be a valid meaningful unique ID.")]
        public Guid Id { get; set; }
    }
}