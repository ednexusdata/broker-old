// Copyright: 2023 Education Nexus Oregon
// Author: Makoa Jacobsen, makoa@makoajacobsen.com

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OregonNexus.Broker.Domain;

namespace OregonNexus.Broker.Data.Configuration;

public class EducationOrganizationConnectorSettingsConfiguration : IEntityTypeConfiguration<EducationOrganizationConnectorSettings>
{
    public void Configure(EntityTypeBuilder<EducationOrganizationConnectorSettings> builder)
    {   
        // Rename ID to UserId
        builder.Property(i => i.Id).HasColumnName("EducationOrganizationConnectorSettingsId");

        // Create unique key constraint for EducationOrganizationid and UserId
        builder.HasIndex(x => new { x.EducationOrganizationId, x.Connector } ).IsUnique();
    }
}
